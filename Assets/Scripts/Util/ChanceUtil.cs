    using System;
    using System.Collections.Generic;
    using Unity.Collections;
    using Object = UnityEngine.Object;
    using Random = System.Random;

    public class ChanceUtil
    {
        private static readonly Random _random = new Random();

        public static double getRandomInt()
        {
            return _random.Next(0, 10001);
        }

        // 오브젝트와 확률을 매핑해서 인자에 넣어주면, 확률 ( 백분율 )에 따라, 결과 값 리턴

        public static Object runChance(Dictionary<Object, double> chanceList)
        {

            Dictionary<Object, double> newChanceList = new Dictionary<Object, double>();
            
            double chanceInt = getRandomInt();

            int totalChance = 0;

            foreach (var key in chanceList.Keys)
            {
                double chance = chanceList[key];
                int fixedChance = (int) Math.Truncate(chance * 100);
                newChanceList.Add(key, totalChance+fixedChance);
                totalChance += fixedChance;
            }

            if (totalChance != 10000)
            {
                throw new Exception("NotEnoughChance");
            }
            
            foreach (var key in newChanceList.Keys)
            {
                if (chanceInt <= newChanceList[key])
                {
                    return key;
                }
            }
            
            throw new Exception("CalculateFailed");
            
        }
    }
