//
//  Author:
//    simon simon@simonwaite.com
//
//  Copyright (c) 2013, Simon Waite
//
//  All rights reserved.
//
//
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimonWaite.Network.Protocols.Irc
{
  public class SynchronizedQueue<T> : INotifyPropertyChanged 
  {
    Queue<T> q = new Queue<T>();

    public SynchronizedQueue()
    {
    }
    public void Clear()
    {
      lock (q)
      {
        q.Clear();
        NotifyPropertyChanged("Clear");
        // clear out subscribers.
        PropertyChanged = null;
      }
    }
    public void Enqueue(T element)
    {
      lock (q)
      {
        q.Enqueue(element);
      }
      NotifyPropertyChanged("Queue");
    }

    public T Dequeue()
    {
      lock (q)
      {
       return q.Dequeue();
      }
    }

    public bool HasMore
    {
      get
      {
        lock (q)
        {
          return q.Count > 0;
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    
    private void NotifyPropertyChanged(String info)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }
  }
}

