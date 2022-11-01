using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using neolink.Vocabulary;

namespace neolink.Helpers
{
    public class PhraseManager
    {
        public List<string> Dictionary;
        public List<string> SessionDictionary;

        string SourceEntity;
        string SourceFeeling;
        string TargetEntity;
        string CurrentFeeling;

        int UpdateCurrentFeelingLimit = 5; // percent chance
        int AddCurrentFeelingPrefixLimit = 50; // percent chance
        int MaxWordLength = 10; // increase to increase word complexity

        public List<int> UniqueKeys;

        public string[] SourceEntities =
        {
            "I",
            "THEY",
            "YOU",
            "SHE",
            "HE"
        };

        public string[] SourceFeelings =
        {
            "HURT",
            "FRIGHTENED",
            "HATED",
            "LOVED",
            "LIKED",
            "DESPISED",
            "SCARED",
            "CAN SEE",
            "HID FROM",
            "CAN HEAR",
            "DON'T KNOW",
            "CAN SEE",
            "CAN'T SEE",
            "CAN'T HELP",
            "CAN HELP",
            "KILLED",
            "ABUSED",
            "NEEDED",
            "HELPED",
            "STOPPED",
            "DIDN'T LIKE",
            "DIDN'T WANT",
            "DIDN'T KNOW",
            "DIDN'T SEE",
            "TRUSTED",
            "RELIED ON",
            "DON'T TRUST",
            "DEPENDED ON",
            "WANTED",
            "MURDERED",
            "WOUNDED",
            "CUT",
            "CHOKED"
        };

        public string[] TargetEntities =
        {
            "ME",
            "YOU",
            "HIM",
            "HER",
            "THEM",
            "ANYONE",
            "PEOPLE",
            "CHILDREN",
            "LIARS",
            "COWARDS",
            "CHEATS"
        };

        public string[] AllFeelingPrefixes =
        {
            "AM",
            "WAS",
            "FELT",
            "FEELING"
        };

        public string[] AllFeelings =
        {
            "HURT",
            "FRIGHTENED",
            "SCARED",
            "UPSET",
            "HAPPY",
            "BAD",
            "SICK",
            "DIZZY",
            "TIRED",
            "STRANGE",
            "SAD",
            "DEPRESSED",
        };

        public string[] ItemPeople =
        {
            "HIS",
            "HER",
            "THEIR",
            "MY"
        };

        public string[] PositiveMovingResponses =
        {
            "MOVING",
            "MORE",
            "WARMER",
            "SHAKE",
            "FEEL",
            "FASTER",
            "MOVE",
            "TURBULANCE"
        };

        public string[] NegativeMovingResponses =
        {
            "STOP",
            "SLOW",
            "CALM",
            "PEACE",
            "REST",
            "FLAT",
            "LEAVE",
            "DIZZY"
        };

        public string[] PositiveStillResponses =
        {
            "HEAR",
            "FEELING",
            "PEACE",
            "HOME",
            "STILL",
            "LEVEL",
            "STAY",
            "REMAIN"
        };

        public string[] NegativeStillResponses =
        {
            "MOVE",
            "SHAKE",
            "RAISE",
            "SWING",
            "TILT",
            "GO",
            "PICK UP",
            "NOTHING"
        };

        public string[] DemoPhrases =
        {
            "SCARED",
            "FEEL LOST",
            "THEY SCARED ME",
            "EVIL",
            "MURDER",
            "SHADOWS",
            "BLOOD",
            "RUN AWAY",
            "NOBODY CARED",
            "I CAN SEE YOU",
            "GET OUT",
            "GO AWAY",
            "HELP",
            "HIS HOME",
            "SHE IS HERE",
            "MY CHILDREN",
            "OLD MAN",
            "TERROR",
            "THE WALLS",
            "UPSTAIRS",
            "THEY HURT HER",
            "SCRATCH",
            "I AM HERE",
            "HE IS HERE",
            "CAN'T SEE",
            "TOO MUCH PAIN"
        };


        public PhraseManager()
        {
            Init();
        }

        public void Init()
        {
            MaxWordLength = 8;
            Dictionary = PopulateDictionary();
            PopulateSessionDictionary();
        }

        public void SetMaxWordLength(int maxLength)
        {
            MaxWordLength = maxLength;

            if (MaxWordLength > 8)
            {
                MaxWordLength = 8;
            }

            if (MaxWordLength < 4)
            {
                MaxWordLength = 4;
            }
        }

        public void AddUniqueKeysFromAlphaNumericString(string input)
        {
            UniqueKeys.Clear();
            string stripped = new string(input.Where(c => char.IsDigit(c)).ToArray());
            if (stripped.Length %2 == 1)
            {
                stripped += '0';
            }

            char [] chars = stripped.ToCharArray();

            //UniqueKeys.Add(Convert.ToInt32(stripped.Substring(0, stripped.Length - 1)));
            //UniqueKeys.Add(Convert.ToInt32(stripped.Substring(1, stripped.Length - 2)));
            for (int i=0; i<chars.Length-1; i+=2)
            {
                string sub = stripped.Substring(i, 2);
                UniqueKeys.Add(Convert.ToInt32(sub));
            }

            for (int i = 1; i < chars.Length/2; i += 2)
            {
                string sub = stripped.Substring(i, 2);
                UniqueKeys.Add(Convert.ToInt32(sub));
            }

            //UniqueKeys.Add(Convert.ToInt32(stripped.Substring(3, stripped.Length - 4)));
            //UniqueKeys.Add(Convert.ToInt32(stripped.Substring(4, stripped.Length - 5)));


            SessionDictionary.Clear();

            foreach (int index in UniqueKeys)
            {
                SessionDictionary.Add(GetWord(index));
                SessionDictionary.Add(GetVerb(index));
                SessionDictionary.Add(GetNoun(index));
                SessionDictionary.Add(GetPreposition(index));
                SessionDictionary.Add(GetPronoun(index));
                SessionDictionary.Add(GetAdjective(index));
                SessionDictionary.Add(GetAdverb(index));
            }

            Console.WriteLine("-------- START UNIQUE KEYS --------");
            foreach(string word in SessionDictionary)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine("-------- END UNIQUE KEYS --------");
        }

        public List<string> PopulateDictionary()
        {
            List<string> dictionary = new List<string>();

            UniqueKeys = new List<int>();

            var assembly = typeof(App).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if (res.Contains("words.txt") || res.Contains("words1.txt"))
                {
                    Stream stream = assembly.GetManifestResourceStream(res);

                    using (var reader = new StreamReader(stream))
                    {
                        string data = "";
                        while ((data = reader.ReadLine()) != null)
                        {
                            dictionary.Add(data);
                        }
                    }
                }
            }

            if (dictionary.Count == 0)
            {
                dictionary.Add("Hello");
                dictionary.Add("Help");
                dictionary.Add("Scared");
                dictionary.Add("Trapped");
            }

            return dictionary;
        }

        public void PopulateSessionDictionary()
        {
            SessionDictionary = new List<string>();
        }

        public string GetDemoPhrase()
        {
            return DemoPhrases[Numbers.GetNextRandom(0, DemoPhrases.Length - 1)].ToLower();
        }

        public string GetPositiveMoveResponse()
        {
            return PositiveMovingResponses[Numbers.GetNextRandom(0, PositiveMovingResponses.Length - 1)].ToLower();
        }

        public string GetNegativeMoveResponse()
        {
            return NegativeMovingResponses[Numbers.GetNextRandom(0, NegativeMovingResponses.Length - 1)].ToLower();
        }

        public string GetPositiveStillResponse()
        {
            return PositiveStillResponses[Numbers.GetNextRandom(0, PositiveStillResponses.Length - 1)].ToLower();
        }

        public string GetNegativeStillResponse()
        {
            return NegativeStillResponses[Numbers.GetNextRandom(0, NegativeStillResponses.Length - 1)].ToLower();
        }

        public string GetWord()
        {
            string word = SessionDictionary[Numbers.GetNextRandom(0, SessionDictionary.Count - 1)].ToLower();

            if (SessionDictionary != null)
            {
                if (Numbers.GetNextRandom(0, 100) > 50)
                {
                    while (word.Length > MaxWordLength)
                    {
                        word = SessionDictionary[Numbers.GetNextRandom(0, SessionDictionary.Count - 1)].ToLower();
                    }
                    return word;
                }
            }

            while (word.Length > MaxWordLength)
            {
                word = DefaultDictionary.Data[Numbers.GetNextRandom(0, DefaultDictionary.Data.Length - 1)].ToLower();
            }

            return word;
        }

        public string GetWord(int index)
        {
            if (index < DefaultDictionary.Data.Length)
            {
                string word = DefaultDictionary.Data[index].ToLower();

                while (word.Length > MaxWordLength)
                {
                    word = DefaultDictionary.Data[Numbers.GetNextRandom(0, DefaultDictionary.Data.Length - 1)].ToLower();
                }

                return word;
            }
            return GetWord();
        }

        public string GetVerb()
        {
            string word = DefaultVerbs.Data[Numbers.GetNextRandom(0, DefaultVerbs.Data.Length - 1)].ToLower();

            while (word.Length > MaxWordLength)
            {
                word = DefaultVerbs.Data[Numbers.GetNextRandom(0, DefaultVerbs.Data.Length - 1)].ToLower();
            }

            return word;
        }

        public string GetVerb(int index)
        {
            if (index < DefaultVerbs.Data.Length)
            {
                string word = DefaultVerbs.Data[index].ToLower();

                while (word.Length > MaxWordLength)
                {
                    word = DefaultVerbs.Data[Numbers.GetNextRandom(0, DefaultVerbs.Data.Length - 1)].ToLower();
                }
                return word;
            }
            return GetVerb();
        }

        public string GetNoun(int index)
        {
            if (index < DefaultNouns.Data.Length)
            {
                string word = DefaultNouns.Data[index].ToLower();

                while (word.Length > MaxWordLength)
                {
                    word = DefaultNouns.Data[Numbers.GetNextRandom(0, DefaultNouns.Data.Length - 1)].ToLower();
                }
                return word;// DefaultNouns[index].ToLower();
            }
            return GetNoun();
        }

        public string GetNoun()
        {
            string word = DefaultNouns.Data[Numbers.GetNextRandom(0, DefaultNouns.Data.Length - 1)].ToLower();

            while (word.Length > MaxWordLength)
            {
                word = DefaultNouns.Data[Numbers.GetNextRandom(0, DefaultNouns.Data.Length - 1)].ToLower();
            }
            return word;
        }

        public string GetPronoun()
        {
            string word = DefaultPronouns.Data[Numbers.GetNextRandom(0, DefaultPronouns.Data.Length - 1)].ToLower();

            while (word.Length > MaxWordLength)
            {
                word = DefaultPronouns.Data[Numbers.GetNextRandom(0, DefaultPronouns.Data.Length - 1)].ToLower();
            }
            return word;
        }

        public string GetPronoun(int index)
        {
            if (index < DefaultPronouns.Data.Length)
            {
                string word = DefaultPronouns.Data[index].ToLower();

                while (word.Length > MaxWordLength)
                {
                    word = DefaultPronouns.Data[Numbers.GetNextRandom(0, DefaultPronouns.Data.Length - 1)].ToLower();
                }
                return word;
            }
            return GetPronoun();
        }

        public string GetAdjective()
        {
            string word = DefaultAdjectives.Data[Numbers.GetNextRandom(0, DefaultAdjectives.Data.Length - 1)].ToLower();

            while (word.Length > MaxWordLength)
            {
                word = DefaultAdjectives.Data[Numbers.GetNextRandom(0, DefaultAdjectives.Data.Length - 1)].ToLower();
            }
            return word;
        }

        public string GetAdjective(int index)
        {
            if (index < DefaultAdjectives.Data.Length)
            {
                string word = DefaultAdjectives.Data[index].ToLower();

                while (word.Length > MaxWordLength)
                {
                    word = DefaultAdjectives.Data[Numbers.GetNextRandom(0, DefaultAdjectives.Data.Length - 1)].ToLower();
                }
                return word;
            }
            return GetAdjective();
        }

        public string GetAdverb()
        {
            string word = DefaultAdverbs.Data[Numbers.GetNextRandom(0, DefaultAdverbs.Data.Length - 1)].ToLower();

            while (word.Length > MaxWordLength)
            {
                word = DefaultAdverbs.Data[Numbers.GetNextRandom(0, DefaultAdverbs.Data.Length - 1)].ToLower();
            }
            return word;
        }

        public string GetAdverb(int index)
        {
            if (index < DefaultAdverbs.Data.Length)
            {
                string word = DefaultAdverbs.Data[index].ToLower();

                while (word.Length > MaxWordLength)
                {
                    word = DefaultAdverbs.Data[Numbers.GetNextRandom(0, DefaultAdverbs.Data.Length - 1)].ToLower();
                }
                return word;// DefaultAdverbs[index].ToLower();
            }
            return GetAdverb();
        }

        public string GetPreposition()
        {
            string word = DefaultPrepositions.Data[Numbers.GetNextRandom(0, DefaultPrepositions.Data.Length - 1)].ToLower();

            while (word.Length > MaxWordLength)
            {
                word = DefaultPrepositions.Data[Numbers.GetNextRandom(0, DefaultPrepositions.Data.Length - 1)].ToLower();
            }
            return word;
        }

        public string GetPreposition(int index)
        {
            if (index < DefaultPrepositions.Data.Length)
            {
                string word = DefaultPrepositions.Data[index].ToLower();

                while (word.Length > MaxWordLength)
                {
                    word = DefaultPrepositions.Data[Numbers.GetNextRandom(0, DefaultPrepositions.Data.Length - 1)].ToLower();
                }
                return word;
            }
            return GetPreposition();
        }


        public string GetSimpleSentence()
        {
            string source = GetSourceEntity();
            string verb1 = GetVerb();
            string noun = GetNoun();

            int type = Numbers.GetNextRandom(0, 100);
            string simpleSentence = source + " " + verb1 + " " + noun;

            if (type >= 0 && type < 33) // first pairing
            {
                simpleSentence = source + " " + verb1;
            }
            else if (type >= 40 && type < 66) // second pairing
            {
                simpleSentence = verb1 + " " + noun;
            }
            return simpleSentence;
        }

        public string GetComplexSentence()
        {
            string source = GetSourceEntity();
            string adverb = GetAdverb();
            string noun = GetNoun();
            string verb1 = GetVerb();
            string adjective = GetAdjective();

            //List<string> funStuff = DefaultNouns.ToList<string>().FindAll(x => x.Contains("TH"));

            return source + " " + adverb + " " + verb1 +  " the " + adjective + " " + noun;
        }

        public string GetItemPeople()
        {
            return ItemPeople[Numbers.GetNextRandom(0, ItemPeople.Length - 1)].ToLower();
        }
        public string GetItem()
        {
            return GetNoun();
        }

        public string GetSombodysItem()
        {
            return GetItemPeople() + " " + GetItem();
        }

        public string GetDescriptiveItem()
        {
            return GetAdjective() + " " + GetItem();
        }

        public string GetFeelingPhrase()
        {
            string feelingPhrase = "";
            SourceEntity = GetSourceEntity();
            SourceFeeling = GetFeeling();
            TargetEntity = GetTargetEntity(SourceEntity);

            feelingPhrase = SourceEntity + " " + SourceFeeling + " " + TargetEntity;
            Console.WriteLine(feelingPhrase);
            return feelingPhrase;
        }

        public string GetSourceEntity()
        {
            SourceEntity = SourceEntities[Numbers.GetNextRandom(0, SourceEntities.Length - 1)].ToLower();

            return SourceEntity.ToLower();
        }

        public string GetFeeling()
        {
            SourceFeeling = SourceFeelings[Numbers.GetNextRandom(0, SourceFeelings.Length - 1)].ToLower();

            return SourceFeeling.ToLower();
        }

        public string GetCurrentFeelingPrefix()
        {
            return AllFeelingPrefixes[Numbers.GetNextRandom(0, AllFeelingPrefixes.Length - 1)].ToLower();
        }

        public void ResetCurrentFeeling()
        {
            CurrentFeeling = GetCurrentFeeling();
        }

        public string GetCurrentFeeling()
        {
            if (CurrentFeeling == null)
            {
                if (Numbers.GetNextRandom(0, 100) < AddCurrentFeelingPrefixLimit)
                {
                    CurrentFeeling = GetCurrentFeelingPrefix() + " " + AllFeelings[Numbers.GetNextRandom(0, AllFeelings.Length - 1)].ToLower();
                }
                else
                {
                    CurrentFeeling = AllFeelings[Numbers.GetNextRandom(0, AllFeelings.Length - 1)].ToLower();
                }
            }

            if (Numbers.GetNextRandom(0, 100) < UpdateCurrentFeelingLimit) // update feeling occasionally
            {
                if (Numbers.GetNextRandom(0, 100) < AddCurrentFeelingPrefixLimit)
                {
                    CurrentFeeling = GetCurrentFeelingPrefix() + " " + AllFeelings[Numbers.GetNextRandom(0, AllFeelings.Length - 1)].ToLower();
                }
                else
                {
                    CurrentFeeling = AllFeelings[Numbers.GetNextRandom(0, AllFeelings.Length - 1)].ToLower();
                }
            }
            return CurrentFeeling.ToLower();
        }


        public string GetTargetEntity(string SourceEntity)
        {
            TargetEntity = TargetEntities[Numbers.GetNextRandom(0, TargetEntities.Length - 1)].ToLower();

            while (SourceEntity == "you" && TargetEntity == "you")
            {
                Console.WriteLine("Correcting target");
                TargetEntity = TargetEntities[Numbers.GetNextRandom(0, TargetEntities.Length - 1)].ToLower();
            }

            while (SourceEntity == "i" && TargetEntity == "me")
            {
                Console.WriteLine("Correcting target");
                TargetEntity = TargetEntities[Numbers.GetNextRandom(0, TargetEntities.Length - 1)].ToLower();
            }

            return TargetEntity.ToLower();
        }
    }
}

