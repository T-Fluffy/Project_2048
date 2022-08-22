using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Block2048 : MonoBehaviour
{
    public int value;
    public Node2048 Node;
    public Block2048 MergingBlock;
    public bool Merging;
    public Vector2 pos => transform.position;

    [SerializeField] private SpriteRenderer _render;
    [SerializeField] private TextMeshPro _text;
    public void Init(BlockType type)
    {
        value = type.Value;
        _render.color = type.Color;
        _text.text = type.Value.ToString(); 
    }
    public void SetBlock(Node2048 node)
    {
        if (Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }
    public void MergeBlock(Block2048 BlockToMergeWith)
    {
        // Set the Block we are Merging with
        MergingBlock = BlockToMergeWith;
        // Set our current node as unoccupied to allow blocks to use it
        Node.OccupiedBlock = null;
        // Set the base block as merging, so it does not get used twice.
        BlockToMergeWith.Merging = true;
    }

    public bool CanMerge(int Value) => value == Value && !Merging && MergingBlock == null;
}
