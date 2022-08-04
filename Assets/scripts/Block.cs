using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Block : MonoBehaviour
{
    public int value;
    public Node Node;
    public Block MergingBlock;
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
    public void SetBlock(Node node)
    {
        if (Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }
    public void MergeBlock(Block BlockToMergeWith)
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
