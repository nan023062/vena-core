// @brief ReusableLinkedList.cs
// @version 1.0
// @author 陈伟
// @data 2024-$CURRENT_MONTH-24
// @Copyright Copyright (c) 2024 陈伟，All rights reserved,

using System;
using System.Collections.Generic;
using XDFramework.Core;
using XDT.Utility;

namespace XDTGame.Core;

public class ReusableLinkedList : ReusableObjectBase
{
    private int size; //链表节点的个数
    private Node head; //头节点

    private static Queue<Node> pool = new Queue<Node>();

    static Node GetNode(Delegate @delegate)
    {
        if (pool.Count > 0)
        {
            var node = pool.Dequeue();
            node.data = @delegate;
            return node;
        }

        return new Node(@delegate);
    }

    static void DisposeNode(Node node)
    {
        if(pool.Count < 1024)
            pool.Enqueue(node);
    }

    public ReusableLinkedList()
    {
        size = 0;
        head = null;
    }

    private class Node
    {
        public Delegate data;
        public Node next;

        public Node(Delegate data)
        {
            this.data = data;
        }
    }

    //在链表头添加元素
    public Delegate AddHead(Delegate obj)
    {
        Node newHead = GetNode(obj);
        if (size == 0)
        {
            head = newHead;
        }
        else
        {
            newHead.next = head;
            head = newHead;
        }

        size++;
        return obj;
    }

    //删除指定的元素，删除成功返回true
    public bool Delete(Delegate value)
    {
        if (size == 0)
        {
            return false;
        }

        Node current = head;
        Node previous = head;
        while (!current.data.Equals(value))
        {
            if (current.next == null)
            {
                return false;
            }
            else
            {
                previous = current;
                current = current.next;
            }
        }

        //如果删除的节点是第一个节点
        if (current == head)
        {
            head = current.next;
            size--;
        }
        else
        {
            //删除的节点不是第一个节点
            previous.next = current.next;
            size--;
        }

        current.data = null;
        current.next = null;
        DisposeNode(current);
        return true;
    }

    private int _invokeCount = 0;

    public void Invoke<T>(T @event)
    {
        if (size == 0) return;

        try
        {
            _invokeCount = 0;

            Node node = head;

            while (node != null)
            {
                (node.data as Action<T>)?.Invoke(@event);
                node = node.next;
                _invokeCount++;

                if (_invokeCount > 100000)
                {
                    DebugSystem.LogError(LogCategory.GameLogic,
                        $"{@event} invoke more than 10000, please check it !!!");
                    return;
                }
            }
        }
        catch (Exception e)
        {
            DebugSystem.LogError(LogCategory.GameLogic, e.Message, e.StackTrace);
        }
    }
    public void Invoke<T,T1>(T @event,T1 @event2)
    {
        if (size == 0) return;

        try
        {
            _invokeCount = 0;

            Node node = head;

            while (node != null)
            {
                (node.data as Action<T,T1>)?.Invoke(@event,event2);
                node = node.next;
                _invokeCount++;

                if (_invokeCount > 100000)
                {
                    DebugSystem.LogError(LogCategory.GameLogic,
                        $"{@event} invoke more than 10000, please check it !!!");
                    return;
                }
            }
        }
        catch (Exception e)
        {
            DebugSystem.LogError(LogCategory.GameLogic, e.Message, e.StackTrace);
        }
    }    //判断链表是否为空
    public bool isEmpty()
    {
        return (size == 0);
    }

    
    
    public override void Deconstruct()
    {
        size = 0;
        Node current = head;

        while (current!=null)
        {
            var before = current;
            current = current.next;
            before.next = null;
            before.data = null;
            DisposeNode(before);
        }

        head = null;
    }

    public override void Construct()
    {
        size = 0;
        head = null;
    }


    public static ReusableLinkedList Create()
    {
        return ReusableObjectFactory.CreateReusableObject<ReusableLinkedList>();
    }
}