﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPAnde.Lib.GA
{
    public static class ChromosomeOperator
    {
        private static ICrossoverOperator crossover;
        private static IMutationOperator mutationOperator;

        public static Type GetCrossoverType { get { return crossover.GetType(); } }
        public static Type GetMutationOperator { get { return mutationOperator.GetType(); } }

        static ChromosomeOperator()
        {
            crossover = new CrosooverOperatorPMX();
            mutationOperator = new MutationOperatorRandomOne();
        }

        public static List<Chromosome> Crossover(List<Gene> firstParent, List<Gene> secondParent, Environment environment)
        {
            return crossover.Crossover(firstParent, secondParent, environment);
        }

        public static void ChangeOperator(ICrossoverOperator cOperator)
        {
            crossover = cOperator;
        }

        public static void ChangeOperator(IMutationOperator mOperator)
        {
            mutationOperator = mOperator;
        }

        public static void Mutation(List<Chromosome> chromosomes, Environment environment)
        {
            mutationOperator.Mutation(chromosomes, environment);
        }
    }

    public class CrossoverOperatorTwoPoints : ICrossoverOperator
    {
        public List<Chromosome> Crossover(List<Gene> first, List<Gene> second, Environment environment)
        {
            var i = Randomizer.Random.Next(1, second.Count);
            var j = Randomizer.Random.Next(i, second.Count);
            List<Gene> s = first.GetRange(i, j - i + 1);
            List<Gene> ms = second.Except(s).ToList();

            List<Chromosome> newChildren = new List<Chromosome>();
            newChildren.Add(new Chromosome(
                                     ms.Take(i)
                                        .Concat(s)
                                        .Concat(ms.Skip(i))
                                        .ToList(), environment));
            return newChildren;
        }
    }

    public class CrosooverOperatorPMX : ICrossoverOperator
    {
        public List<Chromosome> Crossover(List<Gene> first, List<Gene> second, Environment environment)
        {
            var i = Randomizer.Random.Next(1, second.Count);
            var j = Randomizer.Random.Next(i, second.Count);
            
            List<Gene> mFirst = first.GetRange(i, j - i + 1);
            List<Gene> mSecond = second.GetRange(i, j - i + 1);

            //TODO: check skip(j) or j+1
            List<Gene> fFull = first.Take(i)
                .Concat(mSecond)
                .Concat(first.Skip(j+1)).ToList();
            List<Gene> sFull = second.Take(i)
                .Concat(mFirst)
                .Concat(second.Skip(j+1)).ToList();

            var mapper1 = new Dictionary<Gene, Gene>();
            var mapper2 = new Dictionary<Gene, Gene>();
            for (int k = 0; k < mFirst.Count; k++)
            {
                mapper1.Add(mFirst[k], mSecond[k]);
                mapper2.Add(mSecond[k], mFirst[k]);
            }

            for (int k = 0; k < i; k++)
            {
                while (mSecond.Contains(fFull[k]))
                {
                    fFull[k] = mapper2[fFull[k]];
                }
                while (mFirst.Contains(sFull[k]))
                {
                    sFull[k] = mapper1[sFull[k]];
                }
            }
            for (int k = j+1; k < fFull.Count; k++)
            {
                while (mSecond.Contains(fFull[k]))
                {
                    fFull[k] = mapper2[fFull[k]];
                }
                while (mFirst.Contains(sFull[k]))
                {
                    sFull[k] = mapper1[sFull[k]];
                }
            }

            var newChilder = new List<Chromosome>();
            newChilder.Add(new Chromosome(fFull, environment));
            newChilder.Add(new Chromosome(sFull, environment));
            return newChilder;
        }
    }

    public interface ICrossoverOperator
    {
        List<Chromosome> Crossover(List<Gene> first, List<Gene> second, Environment environment);
    }

    public class MutationOperatorRandomOne : IMutationOperator
    {
        public void Mutation(List<Chromosome> chromosomes, Environment Environment)
        {
            foreach (var item in chromosomes)
            {
                int c = 0;
                do{
                    if (Randomizer.Random.NextDouble() < Environment.mutRate)
                    {
                        //random swap
                        int i = Randomizer.Random.Next(0, item.genes.Count);
                        int j = Randomizer.Random.Next(0, item.genes.Count);
                        var tmpGene = item.genes[i];
                        item.genes[i] = item.genes[j];
                        item.genes[j] = tmpGene;
                    }
                    
                }while(++c < Environment.countMut);
                
                
            }
                
        }
    }
    public interface IMutationOperator
    {
        void Mutation(List<Chromosome> chromosomes, Environment Environment);
    }
}

