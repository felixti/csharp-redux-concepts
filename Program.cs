using System;
using System.Collections.Generic;
using System.Linq;

namespace c__redux
{
  class Program
  {
    static void Main(string[] args)
    {
      var store = Store<Product>.CreateStore<ProductAction>(Reducer);

      var unsubscribe = store.Subscribe(() => Console.WriteLine("{0}", Newtonsoft.Json.JsonConvert.SerializeObject(store.GetState())));

      store.Dispatch(new ProductAction { Type = "ADD_PRODUCT", Product = new Product { Id = 1, Name = "MacBook Pro" } });
      store.Dispatch(new ProductAction { Type = "ADD_PRODUCT", Product = new Product { Id = 2, Name = "MacBook Air" } });
      store.Dispatch(new ProductAction { Type = "ADD_PRODUCT", Product = new Product { Id = 3, Name = "MacBook" } });
      store.Dispatch(new ProductAction { Type = "ADD_PRODUCT", Product = new Product { Id = 4, Name = "IMac" } });

      Console.WriteLine("Hello World!");
    }

    static Func<IList<Product>, ProductAction, IList<Product>> Reducer = (products, action) =>
   {
     if (action.Type == "ADD_PRODUCT")
     {
       products.Add(action.Product);
       return products;
     }

     return products;
   };
  }

  public class Product
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }

  public static class Store<TState>
  {

    static IList<TState> State { get; set; }
    static List<Action> Listeners { get; set; }

    public static (
        Func<IEnumerable<TState>> GetState,
        Func<Action, Action> Subscribe,
        Action<TAction> Dispatch)
    CreateStore<TAction>(Func<IList<TState>, TAction, IList<TState>> reducer) where TAction : ReducerAction
    {

      State = new List<TState>();
      Listeners = new List<Action>();

      void Dispatch(TAction action)
      {
        State = reducer(State, action);
        Listeners.ForEach(listener => listener());
      }

      return (GetState, Subscribe, Dispatch);
    }

    static Func<IEnumerable<TState>> GetState = () => State;

    static Func<Action, Action> Subscribe = (action) =>
    {
      Listeners.Add(action);
      Action unsubscribe = () => Listeners.Remove(action);
      return unsubscribe;
    };

  }

  public class ReducerAction
  {
    public string Type { get; set; }
  }

  public class ProductAction : ReducerAction
  {
    public Product Product { get; set; }
  }
}