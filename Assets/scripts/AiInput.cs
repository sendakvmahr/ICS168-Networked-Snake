using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiInput : AbstractInput {

    static GameObject[] foods;

    private Vector2 getDirectionNoFood()
    {
        Vector2 currentDir = GetComponent<Snake>().dir;
        Collider2D coll = Physics2D.OverlapPoint((Vector2)transform.position + currentDir);
        if (coll == null) return currentDir;

        coll = Physics2D.OverlapPoint((Vector2)transform.position + Vector2.up);
        if (coll == null) return Vector2.up;

        coll = Physics2D.OverlapPoint((Vector2)transform.position - Vector2.up);
        if (coll == null) return -Vector2.up;

        coll = Physics2D.OverlapPoint((Vector2)transform.position + Vector2.right);
        if (coll == null) return Vector2.right;

        coll = Physics2D.OverlapPoint((Vector2)transform.position - Vector2.right);
        if (coll == null) return -Vector2.right;
        return Vector2.zero;
    }

    public override Vector2 getDirection()
    {
        foods = GameObject.FindGameObjectsWithTag("Food");
        if (foods.Length == 0) return getDirectionNoFood();

        HashSet<Vector2> visited = new HashSet<Vector2>();
        PriorityQueue queue = new PriorityQueue();

        visited.Add(transform.position);
        queue.add(new Entry((Vector2)transform.position + Vector2.up, Vector2.up, 1));
        visited.Add((Vector2)transform.position + Vector2.up);
        queue.add(new Entry((Vector2)transform.position - Vector2.up, -Vector2.up, 1));
        visited.Add((Vector2)transform.position - Vector2.up);
        queue.add(new Entry((Vector2)transform.position + Vector2.right, Vector2.right, 1));
        visited.Add((Vector2)transform.position + Vector2.right);
        queue.add(new Entry((Vector2)transform.position - Vector2.right, -Vector2.right, 1));
        visited.Add((Vector2)transform.position - Vector2.right);

        while(!queue.isEmpty())
        {
            Entry current = queue.remove();
            Collider2D coll = Physics2D.OverlapPoint(current.position);
            if(coll == null)
            {
                if(!visited.Contains(current.position + Vector2.up))
                {
                    visited.Add(current.position + Vector2.up);
                    queue.add(new Entry(current.position + Vector2.up, current.parent, current.dist + 1));
                }
                if (!visited.Contains(current.position - Vector2.up))
                {
                    visited.Add(current.position - Vector2.up);
                    queue.add(new Entry(current.position - Vector2.up, current.parent, current.dist + 1));
                }
                if (!visited.Contains(current.position + Vector2.right))
                {
                    visited.Add(current.position + Vector2.right);
                    queue.add(new Entry(current.position + Vector2.right, current.parent, current.dist + 1));
                }
                if (!visited.Contains(current.position - Vector2.right))
                {
                    visited.Add(current.position - Vector2.right);
                    queue.add(new Entry(current.position - Vector2.right, current.parent, current.dist + 1));
                }
            }
            else if (coll.name.StartsWith("FoodPrefab"))
            {
                return current.parent;
            }
        }

        return Vector2.zero;
    }

    private class PriorityQueue
    {
        private List<Entry> heap;

        public PriorityQueue()
        {
            heap = new List<Entry>();
        }

        public bool isEmpty()
        {
            return heap.Count == 0;
        }

        public void add(Entry e)
        {
            heap.Add(e);
            shiftUp(heap.Count-1);
        }

        private void shiftUp(int i)
        {
            if(i == 0)return;
            if(heap[i].getWeight() < heap[(i-1)/2].getWeight())
            {
                Entry temp = heap[i];
                heap[i] = heap[(i-1)/2];
                heap[(i-1)/2] = temp;
                shiftUp((i-1)/2);
            }
        }

        public Entry remove()
        {
            Entry e = heap[0];
            heap[0] = heap[heap.Count-1];
            heap.RemoveAt(heap.Count-1);
            shiftDown(0);
            return e;
        }

        private void shiftDown(int i)
        {
            if(i*2+2 < heap.Count)
            {
                if(heap[i*2+2].getWeight() < heap[i*2+1].getWeight())
                {
                    if(heap[i].getWeight() > heap[i*2+2].getWeight())
                    {
                        Entry temp = heap[i];
                        heap[i] = heap[i*2+2];
                        heap[i*2+2] = temp;
                        shiftDown(i*2+2);
                        return;
                    }
                }
            }
            if(i*2+1 < heap.Count)
            {
                if(heap[i].getWeight() > heap[i*2+1].getWeight())
                {
                    Entry temp = heap[i];
                    heap[i] = heap[i*2+1];
                    heap[i*2+1] = temp;
                    shiftDown(i*2+1);
                }
            }
        }
    }

    private class Entry
    {
        public Vector2 position;
        public Vector2 parent;
        public float heuristic;
        public int dist;

        public Entry(Vector2 pos, Vector2 par, int d)
        {
            position = pos;
            parent = par;
            dist = d;
            heuristic = float.MaxValue;
            foreach(GameObject f in foods)
            {
                heuristic = Mathf.Min(heuristic, Mathf.Abs(position.x - f.transform.position.x) + Mathf.Abs(position.y - f.transform.position.y));
            }
        }

        public float getWeight()
        {
            return dist + heuristic;
        }
    }
}
