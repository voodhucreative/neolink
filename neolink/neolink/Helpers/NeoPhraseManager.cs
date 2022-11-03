using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using neolink.Services;
using neolink.Vocabulary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace neolink.Helpers
{
    public class NeoPhraseManager : PhraseManager
    {
        public static int MaxPhrases = 1025;
        public static int MaxIndexes = 128;
        public static int MaxWordsPerPhrase = 4;

        public const int MAX_INDEX_VALUE = 1024;

        public static List<string> SessionPhrases;
        public static List<string> AvailablePhrases;
        public static List<int> PhraseIndexes;

        public static List<int> TriggerIndexStream;
        int TestStreamLength = 500;

        public static Dictionary<int, string> PhrasePairs;

        public static string[] VocabularyData = null;

        static int UseableBanks = 1; // single words
        static int MaxFrequency = 100;
        static int SingleWordFrequency = MaxFrequency;
        static int PhraseFrequency = 0;
        static int SentenceFrequency = 0;

        static int Complexity = 0;
        static int MaxComplexity = 7;

        const int WORDSONLY = 1;
        const int WORDSPHRASES = 2;
        const int WORDSSENTENCES = 3;
        const int WORDSPHRASESSENTENCES = 4;
        const int PHRASESONLY = 5;
        const int PHRASESSENTENCES = 6;
        const int SENTENCESONLY = 7;

        List<int> ReceivedData;

        static PhraseManager DefaultPhraseManager;

        public NeoPhraseManager()
        {
            DefaultPhraseManager = new PhraseManager();
            DefaultPhraseManager.Init();

            SetVocabularyComplexity(1);

            //SetVocabularyOptions(true, false, false);

            BuildAllData();

            // from an embedded json file
            ReceivedData = DataReader.ParseJsonInput("neodata0001.json");
            TriggerIndexStream = ReceivedData;

            // from a user created file
            FileManager.SaveFile("blaa.json", "bla, 23, 34, 324, 2354,er , 635,645,7 ,5 ,34,4,3f24,t6,y3,5h7,g,574,gh,7yh,h537,7h,,745, poo");
            string crap = FileManager.LoadLocalFile("blaa.json");

            FileManager.SaveEmbeddedFile("cack.json", "bla, 23, 34, 324, 2354,er , 635,645,7 ,5 ,34,4,3f24,t6,y3,5h7,g,574,gh,7yh,h537,7h,,745, poo");

            TriggerIndexStream = DataReader.CreateDataListFromInput(crap).Distinct().ToList();

           
            // from random crap
            //TriggerIndexStream = DataReader.CreateDataListFromInput("34ctv,35ty,cq5,ty,25yc2456hvf57fug68,i,h9k4gt6,v2,ctv,c2v5,u3,6y2,45,725v,7uwhtb,3crtfgv,c5,y36,i9h7,0-,9jp80o,67etyfghj.sre5,f6q346f135,67245,y,2456j,5,,35f,46fu,6,u,3246yu2465,u,35,346fu,324,u,f6u3,6fu,46,u74,7,f6,u3,42y7,54,74,f57,uy,h6,h7524,").Distinct().ToList();

            // from raw json
            //TriggerIndexStream = DataReader.CreateDataListFromInput("\"{\n  \"data\": [\n    655,\n    666,\n    123,\n    445,\n    678,\n    12,\n    334,\n    54,\n    876,\n    843,\n    22,\n    567,\n    109,\n    1002,\n    230\n  ]\n}").Distinct().ToList();


            // from simulated
            TriggerIndexStream = GetTriggerIndexStream();
            TriggerIndexStream = DataReader.Sanitize(TriggerIndexStream, MAX_INDEX_VALUE);

            

            Console.WriteLine("Data received");
        }

        public bool UpdateDataStream(string dataStream)
        {
            TriggerIndexStream = DataReader.CreateDataListFromInput(dataStream);
            if(TriggerIndexStream.Count > 0)
            {
                return true;
            }
            return false;
        }


        public static void BuildAllData()
        {
            SetVocabularyOptions(Globals.UseSingleWords, Globals.UsePhrases, Globals.UseSentences);

            VocabularyData = BuildVocabulary();

            while (VocabularyData.Length < MaxPhrases)
            {
                VocabularyData = BuildVocabulary(); // make sure we always have enough
            }

            SessionPhrases = PopulateSessionPhrases();

            PhraseIndexes = PopulatePhraseIndexes();

            //PhraseIndexes = ReceivedData.Distinct().ToList();

            AvailablePhrases = PopulateAvailablePhrases();

            PhrasePairs = PopulatePhrasePairs();
        }

        public static string[] BuildVocabulary()
        {
            string[] vocabData = new string[3000];

            int count = 0;
            while (count < vocabData.Length-1)
            {
                string newPhrase = GetRandomPhrase();
                //int wordCount = newPhrase.Split(' ').Length;

                /*
                while (wordCount > MaxWordsPerPhrase)
                {
                    newPhrase = GetRandomPhrase();
                }*/


                if (Complexity < 5)
                {
                    while (newPhrase.Length > 40)
                    {
                        newPhrase = GetRandomPhrase();
                    }

                    if (newPhrase.Length <= 40)
                    {
                        vocabData[count] = newPhrase;
                        count++;
                    }
                }
                else
                {
                    while (vocabData.Contains(newPhrase))
                    {
                        newPhrase = GetRandomPhrase();
                    }
                }


                vocabData[count] = newPhrase;

                count++;

                Console.WriteLine("Count: " + count);
            }
            return vocabData.Distinct().ToArray();
        }

        public static void SetVocabularyComplexity(int complexity)
        {
            Complexity = complexity;

            if (Complexity > MaxComplexity)
            {
                Complexity = MaxComplexity;
            }

            switch (Complexity)
            {
                case WORDSONLY:
                    SetVocabularyOptions(true, false, false);
                    break;
                case WORDSPHRASES:
                    SetVocabularyOptions(true, true, false);
                    break;
                case WORDSSENTENCES:
                    SetVocabularyOptions(true, false, true);
                    break;
                case WORDSPHRASESSENTENCES:
                    SetVocabularyOptions(true, true, true);
                    break;
                case PHRASESONLY:
                    SetVocabularyOptions(false, true, false);
                    break;
                case PHRASESSENTENCES:
                    SetVocabularyOptions(false, true, true);
                    break;
                case SENTENCESONLY:
                    SetVocabularyOptions(false, false, true);
                    break;
            }
        }

        public static void SetVocabularyOptions(bool useWords, bool usePhrases, bool useSentences)
        {
            Globals.UseSingleWords = useWords;
            Globals.UsePhrases = usePhrases;
            Globals.UseSentences = useSentences;
            UpdateVocabularyFrequencies();
        }

        public static void UpdateVocabularyFrequencies()
        {
            SingleWordFrequency = 0;
            PhraseFrequency = 0;
            SentenceFrequency = 0;

            if (Globals.UseSingleWords) // use single words
            {
                SingleWordFrequency = 100;

                if (Globals.UsePhrases) // and phrases
                {
                    SingleWordFrequency = 50;
                    PhraseFrequency = 50;

                    if (Globals.UseSentences) // and sentences
                    {
                        SingleWordFrequency = 34;
                        PhraseFrequency = 33;
                        SentenceFrequency = 33;
                    }
                }
                else // don't use phrases
                {
                    if (Globals.UseSentences)
                    {
                        SingleWordFrequency = 50;
                        SentenceFrequency = 50;
                    }
                }
            }
            else // don't use single words
            {
                SingleWordFrequency = 0;

                if (Globals.UsePhrases) // use phrases
                {
                    PhraseFrequency = 100;

                    if (Globals.UseSentences) // and sentences
                    {
                        PhraseFrequency = 50;
                        SentenceFrequency = 50;
                    }
                }
                else // don't use phrases
                {
                    PhraseFrequency = 0;

                    if (Globals.UseSentences)
                    {
                        SentenceFrequency = 100;
                    }
                }
            }
        }

        public static string GetRandomPhrase()
        {
            int bank = Numbers.GetNextRandom(0, MaxFrequency);
            int sub_bank = Numbers.GetNextRandom(0, MaxFrequency);
            int phrase_seed = Numbers.GetNextRandom(0, MaxFrequency);

            if (bank >= 0 && bank < SingleWordFrequency)
            {
                // single word
                if (phrase_seed >= 0 && phrase_seed < 20)
                {
                    return DefaultPhraseManager.GetNoun().ToUpper();
                }
                else if (phrase_seed >= 20 && phrase_seed < 40)
                {
                    return DefaultPhraseManager.GetVerb().ToUpper();
                }
                else if (phrase_seed >= 40 && phrase_seed < 60)
                {
                    return DefaultPhraseManager.GetAdverb().ToUpper();
                }
                else if (phrase_seed >= 60 && phrase_seed < 80)
                {
                    return DefaultPhraseManager.GetAdjective().ToUpper();
                }
                return DefaultPhraseManager.GetItem().ToUpper();
            }
            else if (bank >= SingleWordFrequency && bank < PhraseFrequency)
            {
                // phrases
                if (sub_bank >= 0 && sub_bank < 20)
                {
                    return CustomPhrases.Data[Numbers.GetNextRandom(0, CustomPhrases.Data.Length - 1)].ToUpper();
                }
                else if (sub_bank >= 20 && sub_bank < 40)
                {
                    return DefaultPhraseManager.GetFeeling().ToUpper();
                }
                else if (sub_bank >= 40 && sub_bank < 60)
                {
                    return DefaultPhraseManager.GetCurrentFeeling().ToUpper();
                }
                else if (sub_bank >= 60 && sub_bank < 80)
                {
                    return DefaultPhraseManager.GetDemoPhrase().ToUpper();
                }
                else
                {
                    return DefaultPhraseManager.GetDescriptiveItem().ToUpper();
                }
            }
            else // use sentences
            {
                if (phrase_seed >= 0 && phrase_seed < 10)
                {
                    return CustomPhrases.Data[Numbers.GetNextRandom(0, CustomPhrases.Data.Length - 1)].ToUpper();
                }
                else if (phrase_seed >= 10 && phrase_seed < 20)
                {
                    return ConversationalPhrases.Data[Numbers.GetNextRandom(0, ConversationalPhrases.Data.Length - 1)].ToUpper();
                }
                else if (phrase_seed >= 20 && phrase_seed < 30)
                {
                    return FelicitousPhrases.Data[Numbers.GetNextRandom(0, FelicitousPhrases.Data.Length - 1)].ToUpper();
                }
                else if (phrase_seed >= 30 && phrase_seed < 40)
                {
                    return PrepositionBy.Data[Numbers.GetNextRandom(0, PrepositionBy.Data.Length - 1)].ToUpper();
                }
                else if (phrase_seed >= 40 && phrase_seed < 50)
                {
                    return PrepositionWith.Data[Numbers.GetNextRandom(0, PrepositionWith.Data.Length - 1)].ToUpper();
                }
                else if (phrase_seed >= 50 && phrase_seed < 60)
                {
                    return SimplePhrases.Data[Numbers.GetNextRandom(0, SimplePhrases.Data.Length - 1)].ToUpper();
                }
                else if (phrase_seed >= 60 && phrase_seed < 70)
                {
                    return DefaultPhraseManager.GetAdjective().ToUpper();
                }
                else if (phrase_seed >= 70 && phrase_seed < 80)
                {
                    return DefaultPhraseManager.GetFeelingPhrase().ToUpper();
                }
                else if (phrase_seed >= 80 && phrase_seed < 90)
                {
                    return DefaultPhraseManager.GetSombodysItem().ToUpper();
                }
                else
                {
                    return DefaultPhraseManager.GetSimpleSentence().ToUpper();
                }
            }
        }

        public static List<string> PopulateSessionPhrases()
        {
            List<string> phrases = new List<string>();
            string newPhrase = "";

            while(phrases.Count < MaxPhrases)
            {
                newPhrase = GetNeoPhrase();
                //while (phrases.Contains(newPhrase))
                //{
                //    newPhrase = GetNeoPhrase();
                //}
                phrases.Add(newPhrase);
            }
            return phrases;
        }

        public static List<int> PopulatePhraseIndexes()
        {
            List<int> phraseIndexes = new List<int>();
            int newIndex = 0;

            while (phraseIndexes.Count < MaxIndexes)
            {
                newIndex = Numbers.GetNextRandom(0, MAX_INDEX_VALUE);
                while (phraseIndexes.Contains(newIndex))
                {
                    newIndex = Numbers.GetNextRandom(0, MAX_INDEX_VALUE);
                }
                phraseIndexes.Add(newIndex);
            }

            return phraseIndexes;
        }

        public static List<string> PopulateAvailablePhrases()
        {
            List<string> availablePhrases = new List<string>();

            foreach(int index in PhraseIndexes)
            {
                Console.WriteLine("Adding " + SessionPhrases[index] + " (index " + index + ")");
                availablePhrases.Add(SessionPhrases[index]);
            }
            return availablePhrases;//.Distinct().ToList();
        }

        public static Dictionary<int, string> PopulatePhrasePairs()
        {
            Dictionary<int, string> pairs = new Dictionary<int, string>();

            int count = 0;
            foreach(int index in PhraseIndexes)
            {
                pairs.Add(PhraseIndexes[count], AvailablePhrases[count]);
                count++;
            }
            return pairs;

        }

        public static Dictionary<int, string> GetPhrasePairs()
        {
            return PhrasePairs;
        }

        public static string GetNeoPhrase()
        {
            string phrase = VocabularyData[Numbers.GetNextRandom(0, VocabularyData.Length - 1)];
            return phrase;
        }

        public int GetStreamIndex(int currentPosition)
        {
            if (Numbers.GetNextRandom(0, 100) > 50)
            {
                //return -1; // simulate no new number available
            }
            if (currentPosition>=TriggerIndexStream.Count)
            {
                return -1;
            }
            return TriggerIndexStream[currentPosition];
        }

        public bool IsTriggerIndex(int trigger)
        {
            if (PhraseIndexes.Contains(trigger))
            {
                return true;
            }
            return false;
        }

        public string GetAvailablePhrase(int index)
        {
            string found = PhrasePairs.FirstOrDefault(x => x.Key == index).Value;

            if (found != null)
            {
                return found;
            }
            return null;
        }



        public static List<string> GetAvailablePhrases()
        {
            return AvailablePhrases;
        }

        public List<int> GetSimulatedTriggerIndexStream(int length)
        {
            List<int> stream = new List<int>();

            for(int i=0; i<length; i++)
            {
                stream.Add(Numbers.GetNextRandom(0, 1024));
            }
            return stream;
        }

        public List<int> GetTriggerIndexStream()
        {
            TestStreamLength = Numbers.GetNextRandom(250, 1000);
            return GetSimulatedTriggerIndexStream(TestStreamLength);
        }

        public int GetStreamLength()
        {
            return TriggerIndexStream.Count;
        }

        public static string GetTriggeredIndexStream()
        {
            string indexStream = "";

            foreach(int index in TriggerIndexStream)
            {
                indexStream += index + ",";
            }
            return indexStream;
        }
    }
}

