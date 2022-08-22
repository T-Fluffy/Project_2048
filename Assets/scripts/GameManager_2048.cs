using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class GameManager_2048 : MonoBehaviour
{
    [SerializeField] private int _Width = 4;
    [SerializeField] private int _Height = 4;
    [SerializeField] private Node2048 _nodePrefab;
    [SerializeField] private Block2048 _BlockPrefab;
    [SerializeField] private SpriteRenderer _BordPrefab;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private int _WinCondition = 2048;
    [SerializeField] private GameObject _WinScreen, _LoseScreen;
    private List<Node2048> _nodes;
    private List<Block2048> _blocks;
    private GameState _state;
    public int _round;
    public List<Node2048> FreeNodes;
    public List<Node2048> freeNodes = null;
    [SerializeField]private SwipeManager2048 switcher;
    private BlockType GetBlockTypeByValue(int value) => _types.First(t => t.Value == value);
    private void Start()
    {
        ChangeSatet(GameState.GenerateLevel);
    }
    void Update()
    {
        if (_state != GameState.WaitingInput) return;
        if (/*Input.GetKeyDown(KeyCode.LeftArrow)*/  switcher.HorizontalSwipe<0) Shift(Vector2.left);
        if (/*Input.GetKeyDown(KeyCode.RightArrow)*/ switcher.HorizontalSwipe>0) Shift(Vector2.right);
        if (/*Input.GetKeyDown(KeyCode.UpArrow)*/    switcher.VerticalSwipe>0) Shift(Vector2.up);
        if (/*Input.GetKeyDown(KeyCode.DownArrow)*/  switcher.VerticalSwipe<0) Shift(Vector2.down);
    }
    private void ChangeSatet(GameState newState)
    {
        _state = newState;
        switch (newState)
        {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2:1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                _WinScreen.SetActive(true);
                break;
            case GameState.Lose:
                _LoseScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
    void GenerateGrid()
    {
        _round = 0;
        _nodes = new List<Node2048>();
        _blocks = new List<Block2048>();
        for (int x=0 ; x < _Width ; x++ )
        {
            for (int y=0; y < _Height; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
                _nodes.Add(node);
            }
        }
        var center = new Vector2((float)_Width / 2 - 0.5f, (float)_Height / 2 - 0.5f);
        var Board = Instantiate(_BordPrefab,center,Quaternion.identity);
        Board.size=new Vector2(_Width, _Height);
        Camera.main.transform.position = new Vector3(center.x,center.y,-10);
        ChangeSatet(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {
        FreeNodes = _nodes.Where(n => n.OccupiedBlocks == null).OrderBy(b => Random.value).ToList();
        freeNodes.Clear();
        foreach (Node2048 nodes in FreeNodes)
        {
            if (nodes.OccupiedBlock==null)
            {
                freeNodes.Add(nodes);
            }
        }
        foreach (var node in freeNodes.Take(amount))
        {
            SpawnBlock(node, Random.value > 0.8f ? 4 : 2);
        }
       
        if (freeNodes.Count()==1)
        {
            ChangeSatet(GameState.Lose);
            return;
        }
        ChangeSatet(_blocks.Any(b=>b.value == _WinCondition)? GameState.Win:GameState.WaitingInput);
    }
    void SpawnBlock(Node2048 node,int value)
    {
        var Block = Instantiate(_BlockPrefab, node.pos, Quaternion.identity);
        Block.Init(GetBlockTypeByValue(value));
        Block.SetBlock(node);
        _blocks.Add(Block);
    }
    void Shift(Vector2 dir)
    {
        ChangeSatet(GameState.Moving);
        var OrderedBlocks = _blocks.OrderBy(b => b.pos.x).ThenBy(b => b.pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) OrderedBlocks.Reverse();
        foreach (var Block in OrderedBlocks)
        {
            var next = Block.Node;
            do
            {
                Block.SetBlock(next);
                var possibleNode = GetNodeAtPosition(next.pos + dir);
                if (possibleNode!=null)
                {
                    // We know  a node is present
                    // if it's possible to merge, set merge 
                    if (possibleNode.OccupiedBlock!=null && possibleNode.OccupiedBlock.CanMerge(Block.value))
                    {
                        Block.MergeBlock(possibleNode.OccupiedBlock);

                    }
                    // Otherwise, can we move to this spot ?
                    else
                    if (possibleNode.OccupiedBlock == null) next = possibleNode;
                    // None hit? end do while loop .
                }
            } while (next!=Block.Node);
            
        }
        var Sequence = DOTween.Sequence();
        foreach (var Block in OrderedBlocks)
        {
            var movePoint = Block.MergingBlock != null ? Block.MergingBlock.Node.pos : Block.Node.pos;
            Sequence.Insert(0, Block.transform.DOMove(movePoint, _travelTime));

        }
        Sequence.OnComplete(() =>
        {
            foreach (var Block in OrderedBlocks.Where(b=>b.MergingBlock != null))
            {
                MergeBlocks(Block.MergingBlock,Block);
            }
            ChangeSatet(GameState.SpawningBlocks);
        });
        
    }
    void MergeBlocks(Block2048 BaseBlock,Block2048 mergingBlock)
    {
        SpawnBlock(BaseBlock.Node, BaseBlock.value * 2);
        RemoveBlock(BaseBlock);
        RemoveBlock(mergingBlock);
    }
    void RemoveBlock(Block2048 block)
    {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }
    Node2048 GetNodeAtPosition(Vector2 pos)
    {
        return _nodes.FirstOrDefault(n => n.pos == pos);
    }
}
[Serializable]
public struct BlockType
{
    public int Value;
    public Color Color;
}
public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}