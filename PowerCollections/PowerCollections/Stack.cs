using System.Collections;
using System.Collections.Generic;

namespace PowerCollections
{
    public class Stack<T>
    {
        public int Capacity;//размер стека
        private T[] Items; // элементы стека
        private int count;  // количество элементов

        //конструктор
        public Stack(int size)
        {
            Items = new T[size];
            Capacity = size;
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = Count; i > 0; i--)
            {
                yield return Items[i - 1];
            }
        }




        //Проверка на заполненость
        public bool IsEmpty
        {
            get { return count == 0; }
        }
        public bool IsFull
        {
            get { return Items.Length == Capacity; }
        }
        //Размер стека
        public int Count
        {
            get { return count; }
        }
        //Добавление элемента
        internal void Push(T item)
        {
            Items[count++] = item;
        }
        //Снимаю элемент с вершины и возвращаю его значение
        public T Pop()
        {
            return Items[--count];
        }
        //Возвращаю значение элемента на вершине стека, но не извлекаю
        public T Top()
        {
            return Items[count - 1];
        }
    }
}