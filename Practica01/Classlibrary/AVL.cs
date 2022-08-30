﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classlibrary
{
    public class AVL<T>
    {
        public Node<T> root;
        public List<T> TreeList;
        public List<T> SearchTree;
        public int rotations = 0;
        public int height = 0;
        public int NodeCounter { get; set; }
        int counter = 0 ;

        public AVL()
        {
            root = null;
            TreeList = new List<T>();
            SearchTree = new List<T>();
        }


        public Node<T> InsertAVL(Node<T> newNode, Delegate delegate1, Node<T> subArbol)
        {
            Node<T> nuevoPadre = subArbol;
            if (Convert.ToInt32(delegate1.DynamicInvoke(newNode.value, subArbol.value)) < 0)
            {
                if (subArbol.LeftNode == null)
                {
                    subArbol.LeftNode = newNode;
                }
                else
                {
                    subArbol.LeftNode = InsertAVL(newNode, delegate1, subArbol.LeftNode);
                    if ((getBF(subArbol.LeftNode) - getBF(subArbol.RightNode)) == 2)
                    {
                        if (Convert.ToInt32(delegate1.DynamicInvoke(newNode.value, subArbol.LeftNode.value)) < 0)
                        {
                            nuevoPadre = SimpleLR(subArbol);
                        }
                        else
                        {
                            nuevoPadre = rotacionDobleIzquierda(subArbol);
                        }
                    }
                }
            }
            else if (Convert.ToInt32(delegate1.DynamicInvoke(newNode.value, subArbol.value)) > 0)
            {
                if (subArbol.RightNode == null)
                {
                    subArbol.RightNode = newNode;
                }
                else
                {
                    subArbol.RightNode = InsertAVL(newNode, delegate1, subArbol.RightNode);
                    if ((getBF(subArbol.RightNode) - getBF(subArbol.LeftNode)) == 2)
                    {
                        if (Convert.ToInt32(delegate1.DynamicInvoke(newNode.value, subArbol.RightNode.value)) > 0)
                        {
                            nuevoPadre = SimpleRR(subArbol);
                        }
                        else
                        {
                            nuevoPadre = rotacionDobleDerecha(subArbol);
                        }
                    }
                }
            }
            else
            {
            }
            //Actualizar el FE
            if ((subArbol.LeftNode == null) && (subArbol.RightNode != null))
            {
                subArbol.BF = subArbol.RightNode.BF + 1;
            }
            else if ((subArbol.RightNode == null) && (subArbol.LeftNode != null))
            {
                subArbol.BF = subArbol.LeftNode.BF + 1;
            }
            else
            {
                subArbol.BF = Math.Max(getBF(subArbol.LeftNode), getBF(subArbol.RightNode)) + 1;
            }
            height = getHeight(subArbol);
            return nuevoPadre;
        }

        public void Insert(T data, Delegate delegate1)
        {
            Node<T> newNode = new Node<T>();
            newNode.value = data;
            newNode.LeftNode = null;
            newNode.RightNode = null;
            if (root == null)
            {
                root = newNode;
            }
            else
            {
                root = InsertAVL(newNode, delegate1, root);
            }
            InOrden(root);
        }

        public void Delete(T data, Delegate delegate1)
        {
            root = DeleteAVL(root, delegate1, data);
            InOrden(root);
        }

        private Node<T> DeleteAVL(Node<T> actual, Delegate delegate1, T data)
        {
            if (actual == null)
            {
                return actual;
            }

            if (Convert.ToInt32(delegate1.DynamicInvoke(data, actual.value)) < 0)
            {
                actual.LeftNode = DeleteAVL(actual.LeftNode, delegate1, data);
            }
            else if (Convert.ToInt32(delegate1.DynamicInvoke(data, actual.value)) > 0)
            {
                actual.RightNode = DeleteAVL(actual.RightNode, delegate1, data);
            }
            else
            {
                //El nodo es igual al elemento y se elimina
                //Node con un único hijo o es hoja
                if ((actual.LeftNode == null) || (actual.RightNode == null))
                {
                    Node<T> temp = null;
                    if (temp == actual.LeftNode)
                    {
                        temp = actual.RightNode;
                    }
                    else
                    {
                        temp = actual.LeftNode;
                    }

                    //No tiene hijos
                    if (temp == null)
                    {
                        actual = null; //Se elmina poniéndolo en null
                    }
                    else
                    {
                        actual = temp; //Elimina el valor actual reemplazándolo por su hijo
                    }
                }
                else
                {
                    //Node con dos hijos, se busca el predecesor
                    Node<T> temp = MaxNode(actual.LeftNode);

                    //Se copia el dato del predecesor
                    actual.value = temp.value;

                    //Se elimina el predecesor
                    actual.LeftNode = DeleteAVL(actual.LeftNode, delegate1, temp.value);
                }
            }
            //Si solo tiene un Node
            if (actual == null)
            {
                return actual;
            }

            //Actualiza altura
            actual.BF = Math.Max(getHeight(actual.LeftNode), getHeight(actual.RightNode)) + 1;

            int FE = getFE2(actual);
            height = FE;
            if (FE > 1 && getFE2(actual.LeftNode) >= 0)
            {
                return SimpleRR(actual);
            }

            if (FE < -1 && getFE2(actual.RightNode) <= 0)
            {
                return SimpleLR(actual);
            }

            if (FE > 1 && getFE2(actual.LeftNode) < 0)
            {
                actual.LeftNode = SimpleLR(actual.LeftNode);
                return SimpleRR(actual);
            }

            if (FE < -1 && getFE2(actual.RightNode) > 0)
            {
                actual.RightNode = SimpleRR(actual.RightNode);
                return SimpleLR(actual);
            }
            return actual;
        }

        public int getBF(Node<T> X)
        {

            if (X == null)
            {
                return -1;
            }
            else
            {
                return X.BF;
            }
        }
        public Node<T> SimpleLR(Node<T> A)
        {
            rotations++;
            Node<T> aux = A.LeftNode;
            A.LeftNode = aux.RightNode;
            aux.RightNode = A;
            A.BF = Math.Max(getBF(A.LeftNode), getBF(A.RightNode)) + 1;
            aux.BF = Math.Max(getBF(aux.LeftNode), getBF(aux.RightNode)) + 1;
            Cantrotations(rotations);
            return aux;
        }

        public Node<T> SimpleRR(Node<T> A)
        {
            rotations++;
            Node<T> aux = A.RightNode;
            A.RightNode = aux.LeftNode;
            aux.LeftNode = A;
            A.BF = Math.Max(getBF(A.LeftNode), getBF(A.RightNode)) + 1;
            aux.BF = Math.Max(getBF(aux.LeftNode), getBF(aux.RightNode)) + 1;
            Cantrotations(rotations);
            return aux;
        }

        //Cantidad de rotations
        public int Cantrotations(int num)
        {
            return num;
        }

        public Node<T> rotacionDobleIzquierda(Node<T> A)
        {
            rotations++;
            Node<T> aux;
            A.LeftNode = SimpleRR(A.LeftNode);
            aux = SimpleLR(A);
            Cantrotations(rotations);
            return aux;
        }

        public Node<T> rotacionDobleDerecha(Node<T> A)
        {
            rotations++;
            Node<T> aux;
            A.RightNode = SimpleLR(A.RightNode);
            aux = SimpleRR(A);
            Cantrotations(rotations);
            return aux;
        }

        

        private int getFE2(Node<T> actual)
        {
            if (actual == null)
            {
                return 0;
            }

            return getHeight(actual.LeftNode) - getHeight(actual.RightNode);
        }

        

        //Altura del árbol
        private int getHeight(Node<T> actual)
        {
            if (actual == null)
            {
                return 0;
            }

            return actual.BF;
        }

        private Node<T> MaxNode(Node<T> Node)
        {
            Node<T> actual = Node;
            while (actual.RightNode != null)
            {
                actual = actual.RightNode;
            }

            return actual;
        }

        public void Search(Node<T> SearchNode, Delegate delegate1)
        {
            SearchTree.Clear();
            Node<T> aux = ReSearch(root, delegate1, SearchNode);
            if (aux == null || SearchNode.value == null)
            {
                SearchTree.Clear();
                //ListaCUI.Clear();
            }
            else
            {
                SearchTree.Add(ReSearch(root, delegate1, SearchNode).value);
                //ListaCUI.Add(busqueda2(raiz, delegado1, buscado).Valor);
            }
        }

        public Node<T> ReSearch(Node<T> aux, Delegate delegate1, Node<T> Search)
        {
            Node<T> Output = null;
            if (aux == null || Search.value == null)
            {
                Output = null;
            }
            else
            {
                if (Convert.ToInt32(delegate1.DynamicInvoke(aux.value, Search.value)) == 0)
                {
                    Output = aux;
                }
                else
                {
                    if (Convert.ToInt32(delegate1.DynamicInvoke(Search.value, aux.value)) < 0)
                    {
                        Output = ReSearch(aux.LeftNode, delegate1, Search);
                    }
                    else
                    {
                        if (Convert.ToInt32(delegate1.DynamicInvoke(Search.value, aux.value)) > 0)
                        {
                            Output = ReSearch(aux.RightNode, delegate1, Search);
                        }
                    }
                }
            }
            return Output;
        }


        public void Recorrido(Node<T> r)
        {

            if (r != null)
            {
                Recorrido(r.LeftNode);
                TreeList.Add(r.value);
                Recorrido(r.RightNode);
            }

        }

        public void InOrden(Node<T> raiz)
        {
            TreeList.Clear();
            Recorrido(raiz);
        }
        
    }
}
