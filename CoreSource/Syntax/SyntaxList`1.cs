﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// A list of <see cref="SyntaxNode"/>.
    /// </summary>
    public partial struct SyntaxList<TNode> : IReadOnlyList<TNode>, IEquatable<SyntaxList<TNode>>
        where TNode : SyntaxNode
    {
        private readonly SyntaxNode node;

        internal SyntaxList(SyntaxNode node)
        {
            this.node = node;
        }

        internal SyntaxNode Node
        {
            get
            {
                return this.node;
            }
        }

        /// <summary>
        /// The number of nodes in the list.
        /// </summary>
        public int Count
        {
            get
            {
                return node == null ? 0 : (node.IsList ? node.SlotCount : 1);
            }
        }

        /// <summary>
        /// Gets the node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the node to get or set.</param>
        /// <returns>The node at the specified index.</returns>
        public TNode this[int index]
        {
            get
            {
                if (node != null)
                {
                    if (node.IsList)
                    {
                        if (unchecked((uint)index < (uint)node.SlotCount))
                        {
                            return (TNode)node.GetNodeSlot(index);
                        }
                    }
                    else if (index == 0)
                    {
                        return (TNode)node;
                    }
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        internal SyntaxNode ItemInternal(int index)
        {
            if (node.IsList)
            {
                return node.GetNodeSlot(index);
            }

            Debug.Assert(index == 0);
            return node;
        }

        /// <summary>
        /// The absolute span of the list elements in characters, not including the leading and trailing trivia of the first and last elements.
        /// </summary>
        public TextSpan FullSpan
        {
            get
            {
                if (this.Count == 0)
                {
                    return default(TextSpan);
                }
                else
                {
                    return TextSpan.FromBounds(this[0].FullSpan.Start, this[this.Count - 1].FullSpan.End);
                }
            }
        }

        /// <summary>
        /// The absolute span of the list elements in characters, including the leading and trailing trivia of the first and last elements.
        /// </summary>
        public TextSpan Span
        {
            get
            {
                if (this.Count == 0)
                {
                    return default(TextSpan);
                }
                else
                {
                    return TextSpan.FromBounds(this[0].Span.Start, this[this.Count - 1].Span.End);
                }
            }
        }

        /// <summary>
        /// Returns the string representation of the nodes in this list, not including 
        /// the first node's leading trivia and the last node's trailing trivia.
        /// </summary>
        /// <returns>
        /// The string representation of the nodes in this list, not including 
        /// the first node's leading trivia and the last node's trailing trivia.
        /// </returns>
        public override string ToString()
        {
            return this.node != null ? this.node.ToString() : string.Empty;
        }

        /// <summary>
        /// Returns the full string representation of the nodes in this list including 
        /// the first node's leading trivia and the last node's trailing trivia.
        /// </summary>
        /// <returns>
        /// The full string representation of the nodes in this list including 
        /// the first node's leading trivia and the last node's trailing trivia.
        /// </returns>
        public string ToFullString()
        {
            return this.node != null ? this.node.ToFullString() : string.Empty;
        }

        /// <summary>
        /// Creates a new list with the specified node added at the end.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public SyntaxList<TNode> Add(TNode node)
        {
            return this.Insert(this.Count, node);
        }

        /// <summary>
        /// Creates a new list with the specified nodes added at the end.
        /// </summary>
        /// <param name="nodes">The nodes to add.</param>
        public SyntaxList<TNode> AddRange(IEnumerable<TNode> nodes)
        {
            return this.InsertRange(this.Count, nodes);
        }

        /// <summary>
        /// Creates a new list with the specified node inserted at the index.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="node">The node to insert.</param>
        public SyntaxList<TNode> Insert(int index, TNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return InsertRange(index, new[] { node });
        }

        /// <summary>
        /// Creates a new list with the specified nodes inserted at the index.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="nodes">The nodes to insert.</param>
        public SyntaxList<TNode> InsertRange(int index, IEnumerable<TNode> nodes)
        {
            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            var list = this.ToList();
            list.InsertRange(index, nodes);

            if (list.Count == 0)
            {
                return this;
            }
            else
            {
                return CreateList(list[0].Green, list);
            }
        }

        /// <summary>
        /// Creates a new list with the element at specified index removed.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        public SyntaxList<TNode> RemoveAt(int index)
        {
            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return this.Remove(this[index]);
        }

        /// <summary>
        /// Creates a new list with the element removed.
        /// </summary>
        /// <param name="node">The element to remove.</param>
        public SyntaxList<TNode> Remove(TNode node)
        {
            return CreateList(this.Where(x => x != node).ToList());
        }

        /// <summary>
        /// Creates a new list with the specified element replaced with the new node.
        /// </summary>
        /// <param name="nodeInList">The element to replace.</param>
        /// <param name="newNode">The new node.</param>
        public SyntaxList<TNode> Replace(TNode nodeInList, TNode newNode)
        {
            return ReplaceRange(nodeInList, new[] { newNode });
        }

        /// <summary>
        /// Creates a new list with the specified element replaced with new nodes.
        /// </summary>
        /// <param name="nodeInList">The element to replace.</param>
        /// <param name="newNodes">The new nodes.</param>
        public SyntaxList<TNode> ReplaceRange(TNode nodeInList, IEnumerable<TNode> newNodes)
        {
            if (nodeInList == null)
            {
                throw new ArgumentNullException("nodeInList");
            }

            if (newNodes == null)
            {
                throw new ArgumentNullException("newNodes");
            }

            var index = this.IndexOf(nodeInList);
            if (index >= 0 && index < this.Count)
            {
                var list = this.ToList();
                list.RemoveAt(index);
                list.InsertRange(index, newNodes);
                return CreateList(list);
            }
            else
            {
                throw new ArgumentException("nodeInList");
            }
        }

        private static SyntaxList<TNode> CreateList(List<TNode> items)
        {
            if (items.Count == 0)
            {
                return default(SyntaxList<TNode>);
            }
            else
            {
                return CreateList(items[0].Green, items);
            }
        }

        private static SyntaxList<TNode> CreateList(GreenNode creator, List<TNode> items)
        {
            if (items.Count == 0)
            {
                return default(SyntaxList<TNode>);
            }

            var newGreen = creator.CreateList(items.Select(n => n.Green));
            return new SyntaxList<TNode>(newGreen.CreateRed());
        }

        /// <summary>
        /// The first node in the list.
        /// </summary>
        public TNode First()
        {
            return this[0];
        }

        /// <summary>
        /// The first node in the list or default if the list is empty.
        /// </summary>
        public TNode FirstOrDefault()
        {
            if (this.Any())
            {
                return this[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The last node in the list.
        /// </summary>
        public TNode Last()
        {
            return this[this.Count - 1];
        }

        /// <summary>
        /// The last node in the list or default if the list is empty.
        /// </summary>
        public TNode LastOrDefault()
        {
            if (this.Any())
            {
                return this[this.Count - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// True if the list has at least one node.
        /// </summary>
        public bool Any()
        {
            Debug.Assert(node == null || Count != 0);
            return node != null;
        }

        // for debugging
        private TNode[] Nodes
        {
            get { return this.ToArray(); }
        }

        /// <summary>
        /// Get's the enumerator for this list.
        /// </summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator()
        {
            if (this.Any())
            {
                return new EnumeratorImpl(this);
            }

            return SpecializedCollections.EmptyEnumerator<TNode>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.Any())
            {
                return new EnumeratorImpl(this);
            }

            return SpecializedCollections.EmptyEnumerator<TNode>();
        }

        public static bool operator ==(SyntaxList<TNode> left, SyntaxList<TNode> right)
        {
            return left.node == right.node;
        }

        public static bool operator !=(SyntaxList<TNode> left, SyntaxList<TNode> right)
        {
            return left.node != right.node;
        }

        public bool Equals(SyntaxList<TNode> other)
        {
            return node == other.node;
        }

        public override bool Equals(object obj)
        {
            return obj is SyntaxList<TNode> && Equals((SyntaxList<TNode>)obj);
        }

        public override int GetHashCode()
        {
            return this.node != null ? this.node.GetHashCode() : 0;
        }

        public static implicit operator SyntaxList<TNode>(SyntaxList<SyntaxNode> nodes)
        {
            return new SyntaxList<TNode>(nodes.node);
        }

        public static implicit operator SyntaxList<SyntaxNode>(SyntaxList<TNode> nodes)
        {
            return new SyntaxList<SyntaxNode>(nodes.Node);
        }

        /// <summary>
        /// The index of the node in this list, or -1 if the node is not in the list.
        /// </summary>
        public int IndexOf(TNode node)
        {
            var index = 0;
            foreach (var child in this)
            {
                if (object.Equals(child, node))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public int IndexOf(Func<TNode, bool> predicate)
        {
            var index = 0;
            foreach (var child in this)
            {
                if (predicate(child))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public int LastIndexOf(TNode node)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (object.Equals(this[i], node))
                {
                    return i;
                }
            }

            return -1;
        }

        public int LastIndexOf(Func<TNode, bool> predicate)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (predicate(this[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}