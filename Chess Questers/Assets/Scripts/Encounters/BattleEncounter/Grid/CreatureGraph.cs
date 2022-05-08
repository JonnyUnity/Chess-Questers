using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureGraph
{
    private Dictionary<Creature, List<WeightedEdge>> adjList;

    public CreatureGraph()
    {
        adjList = new Dictionary<Creature, List<WeightedEdge>>();
    }
        

    public void AddEdge(Creature source, Creature destination, int weight)
    {
        if (adjList.ContainsKey(source))
        {
            adjList[source].Add(new WeightedEdge(source, destination, weight));
        }
        else
        {
            adjList.Add(source, new List<WeightedEdge> { new WeightedEdge(source, destination, weight) });
        }
    }

    public void UpdateEdge(Creature source, Creature destination, int weight)
    {
        if (adjList.ContainsKey(source))
        {
            var edges = adjList[source];
            var edgeToUpdate = edges.Where(w => w.End == destination).SingleOrDefault();

            if (edgeToUpdate == null)
            {
                AddEdge(source, destination, weight);
            }
            else
            {
                edgeToUpdate.Weight = weight;
            }


        }
    }

    public void DebugEdges(Creature source)
    {
        if (adjList.ContainsKey(source))
        {
            foreach (WeightedEdge edge in adjList[source])
            {
                Debug.Log(edge.End.Name + " - " + edge.Weight);
            }
        }
    }
        

    public void DebugAll()
    {
        foreach (Creature creature in adjList.Keys)
        {
            Debug.Log(creature.Name + " - " + creature.X + "," + creature.Y);
            foreach (WeightedEdge edge in adjList[creature])
            {
                Debug.Log(edge.End.Name + " - " + edge.Weight);
            }
        }
    }



}

public class Vertex<T>
{

    //private List<Vertex<T>> _neighbours;
    //private List<WeightedEdge<T>> _edges;

    public T Value { get; set; }

    public Vertex(T value)
    {
        Value = value;
        //_neighbours = new List<Vertex<T>>();
        //_edges = new List<WeightedEdge<T>>();
    }


    //public void AddNeighbour(Vertex<T> vertex)
    //{
    //    _neighbours.Add(vertex);
    //}

    //public void AddEdge(WeightedEdge<T> edge)
    //{
    //    _edges.Add(edge);
    //}


}

public class WeightedEdge
{
    private int _weight;
    private Creature _start;
    private Creature _end;

    public int Weight { get; set; }
    public Creature Start { get { return _start; } }
    public Creature End { get { return _end; } }

    public WeightedEdge(Creature start, Creature end, int weight)
    {
        _start = start;
        _end = end;
        Weight = weight;
    }



}    