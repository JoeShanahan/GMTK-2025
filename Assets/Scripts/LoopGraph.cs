using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gmtk2025
{
    [Serializable]
    public class LoopGraph
    {
        [Serializable]
        public class LoopData
        {
            public bool IsRoot = false;
            public float Radius = 1f;
            public Vector2 Position;

            public List<ConnectorData> Connectors = new();
        }

        [Serializable]
        public class ConnectorData
        {
            public ConnectorType Type;
            public int Value;
            public float ParentLoopSpace;

            private LoopData parentLoop;
            private LoopData childLoop;

            public List<LoopData> Loops => new() { parentLoop, childLoop }; 
        }

        private List<LoopData> _loops;
        private List<LoopData> _roots = new();

        public List<LoopData> Roots => _roots;

        public void AddRoot(LoopData loop)
        {
            if (loop.IsRoot == false)
                return;

            _loops.Add(loop);
            _roots.Add(loop);
        }

        public void ConnectNewLoop(LoopData newLoop, LoopData targetLoop, ConnectorData connector)
        {
            if (!_loops.Contains(targetLoop))
                return;

            _loops.Add(newLoop);
        }

        public List<LoopData> GetConnectedLoops(LoopData startingLoop, out LoopData rootLoop)
        {
            rootLoop = startingLoop.IsRoot ? startingLoop : null;
            List<LoopData> output = new();

            if (!_loops.Contains(startingLoop))
                return output;

            // We will find all of the connected loops using a breadth-first search

            Dictionary<LoopData, bool> visitedLoops = new(_loops.Count);
            visitedLoops[startingLoop] = true;

            Queue<LoopData> loopExplorationQueue = new(_loops.Count);
            loopExplorationQueue.Enqueue(startingLoop);

            while (loopExplorationQueue.Count > 0)
            {
                LoopData currentLoop = loopExplorationQueue.Dequeue();

                foreach (var discoveredLoop in GetNeighbouringLoops(currentLoop))
                {
                    if (visitedLoops[discoveredLoop] == false)
                    {
                        output.Add(discoveredLoop);

                        if (discoveredLoop.IsRoot == true)
                            rootLoop = discoveredLoop;

                        loopExplorationQueue.Enqueue(discoveredLoop);
                        visitedLoops[discoveredLoop] = true;
                    }
                }
            }

            return output;
        }

        public List<LoopData> GetNeighbouringLoops(LoopData loop)
        {
            List<LoopData> output = new();

            if (!_loops.Contains(loop)) 
                return output;

            foreach (ConnectorData edge in loop.Connectors)
            {
                foreach (LoopData potentialNeighbour in edge.Loops)
                {
                    if (potentialNeighbour != loop && potentialNeighbour != null)
                        output.Add(potentialNeighbour);
                }
            }

            return output;
        }

        public void RemoveRoot(LoopData root)
        {
            if (root.IsRoot == false)
                return;

            var ConnectedLoops = GetConnectedLoops(root, out _);

            _loops.RemoveAll(l => ConnectedLoops.Contains(l));
            _loops.Remove(root);
        }
    }
}
