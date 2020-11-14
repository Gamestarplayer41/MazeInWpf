using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Documents;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    public class AlgorithmManager
    {
        private List<Tuple<string, Thread>> Threads = new List<Tuple<string, Thread>>();

        private List<Tuple<string, IAlgorithm>> Algorithms = new List<Tuple<string, IAlgorithm>>();

        public string[] Alorithms = new string[1] {"wallfollower"};

        public AlgorithmManager()
        {
            Algorithms.Add(new Tuple<string, IAlgorithm>("wallfollower", new Wallfollower()));
        }

        public bool injectMaze(string algorithmName, Maze maze)
        {
            foreach (var algorithm in Algorithms)
            {
                if (algorithm.Item1.Equals(algorithmName))
                {
                    algorithm.Item2.injectMaze(maze);
                    return true;
                }
            }

            return false;
        }

        private Tuple<string, IAlgorithm> findAlgorithm(string algorithmName)
        {
            foreach (var algorithm in Algorithms)
                if (algorithm.Item1.Equals(algorithmName))
                    return algorithm;
            return null;
        }

        private Tuple<string, Thread> findThread(string algorithmName)
        {
            foreach (var thread in Threads)
                if (thread.Item1 == algorithmName)
                    return thread;
            return null;
        }

        public bool assignThread(string algorithmName)
        {
            Tuple<string, IAlgorithm> algorithm = this.findAlgorithm(algorithmName);
            if (algorithm == null)
                return false;
            Tuple<string, Thread> thread = this.findThread(algorithmName);
            if (thread != null)
            {
                Threads.Remove(thread);
            }
            Threads.Add(new Tuple<string, Thread>(algorithmName, new Thread(algorithm.Item2.SolveMaze)));
            return true;
        }

        public bool startThread(string algorithmName)
        {
            Tuple<string, IAlgorithm> algorithm = this.findAlgorithm(algorithmName);
            if (algorithm == null)
                return false;
            Tuple<string, Thread> thread = this.findThread(algorithmName);
            thread.Item2.Start();
            return true;
        }

        public bool stopThread(string algorithmName)
        {
            Tuple<string, IAlgorithm> algorithm = this.findAlgorithm(algorithmName);
            if (algorithm == null)
                return false;
            algorithm.Item2.stopThread = true;
            return true;
        }

        public bool stopAllThreads()
        {
            
        }
    }
}