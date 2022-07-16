using shared._2d;
using System.Numerics;

CFTree cFTree = new CFTree();
cFTree.BranchingFactor = 4;
cFTree.NumberOfEntries = 2;
cFTree.Threshold = 1.5f;

cFTree.Add(new Vector2(0, 1));
cFTree.Add(new Vector2(1, 3));
cFTree.Add(new Vector2(3, 5));
cFTree.Add(new Vector2(4, 1));
cFTree.Add(new Vector2(6, 4));

cFTree.Add(new Vector2(3, 3));
cFTree.Add(new Vector2(1, 1));
cFTree.Add(new Vector2(3, 7));
cFTree.Add(new Vector2(5, 4));
cFTree.Add(new Vector2(0, 0));

cFTree.Add(new Vector2(2, 4));
cFTree.Add(new Vector2(0, 2));
cFTree.Add(new Vector2(1, 5));
cFTree.Add(new Vector2(4, 2));
cFTree.Add(new Vector2(5, 4));

cFTree.Add(new Vector2(7, 5));
cFTree.Add(new Vector2(0, 1));
cFTree.Add(new Vector2(3, 6));
cFTree.Add(new Vector2(1, 1));
cFTree.Add(new Vector2(1, 0));


var r = cFTree.Clustering();

Console.WriteLine("Hello");
