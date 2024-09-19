using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    public class Node
    {
        public string Value;
        public List<Node> Children;

        public Node(string value)
        {
            Value = value;
            Children = new List<Node>();
        }
    }

    static void Main(string[] args)
    {
        string filePath = @"C:\Users\escal\Downloads\lenguajelab2.txt";

        try
        {
            // Leer todas las cadenas del archivo de texto
            string[] cadenas = File.ReadAllLines(filePath);
            if (cadenas.Length < 50)
            {
                Console.WriteLine("El archivo debe contener al menos 50 cadenas.");
                return;
            }

            // Tabla de transiciones general
            Dictionary<string, Dictionary<char, string>> tablaTransicionesGeneral = new Dictionary<string, Dictionary<char, string>>();

            // Procesar las primeras 50 cadenas
            for (int i = 0; i < 50; i++)
            {
                string cadena = cadenas[i].Trim();

                // Verificar que la cadena tenga entre 6 y 20 caracteres
                if (cadena.Length < 6 || cadena.Length > 20)
                {
                    Console.WriteLine($"Cadena {i + 1} no válida (longitud fuera de rango): '{cadena}'\n");
                    continue;
                }

                Console.WriteLine($"\nProcesando cadena {i + 1}: '{cadena}'\n");

                // Inicializar las pilas
                Stack<char> pilaAceptados = new Stack<char>();
                Stack<char> pilaErrores = new Stack<char>();
                Stack<string> pilaEstados = new Stack<string>();

                // Inicializar el árbol de derivación
                Node root = new Node("q0");

                // Verificar si la cadena es válida y generar las transiciones
                bool esValida = VerificarCadena(cadena, root, tablaTransicionesGeneral, pilaAceptados, pilaErrores, pilaEstados);

                // Mostrar el árbol de derivación
                Console.WriteLine("Árbol de derivación:");
                PrintTree(root, "", true);

                // Mostrar las pilas
                Console.WriteLine("\nPila de caracteres aceptados: " + string.Join(", ", pilaAceptados));
                Console.WriteLine("Pila de errores: " + string.Join(", ", pilaErrores));
                Console.WriteLine("Pila de estados totales: " + string.Join(", ", pilaEstados));

                // Mostrar el resultado
                if (esValida)
                {
                    Console.WriteLine($"\nLa cadena '{cadena}' es válida según el autómata.");
                }
                else
                {
                    Console.WriteLine($"\nLa cadena '{cadena}' no es válida según el autómata.");
                }

                Console.WriteLine(new string('-', 50));
            }

            // Mostrar la tabla de transición general
            Console.WriteLine("\nTabla de transición general:");
            PrintTransitionTable(tablaTransicionesGeneral);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }
    }

    static bool VerificarCadena(string cadena, Node root, Dictionary<string, Dictionary<char, string>> tablaTransicionesGeneral,
                            Stack<char> pilaAceptados, Stack<char> pilaErrores, Stack<string> pilaEstados)
{
    string estadoActual = "q0";  // Estado inicial
    Node currentNode = root;
    bool esValida = false;  // Variable para verificar si la cadena es válida

    foreach (char caracter in cadena)
    {
        string estadoSiguiente = null;
        pilaEstados.Push(estadoActual);  // Guardar el estado actual en la pila de estados

        switch (estadoActual)
        {
            case "q0":
                if (caracter == 'a')
                {
                    estadoSiguiente = "q1";
                }
                break;

            case "q1":
                if (caracter == 'b')
                {
                    estadoSiguiente = "q2";
                }
                break;

            case "q2":
                if (caracter == 'a')
                {
                    estadoSiguiente = "q3";
                }
                break;

            case "q3":
                if (caracter == 'a')
                {
                    estadoSiguiente = "q3";
                }
                else if (caracter == 'b')
                {
                    estadoSiguiente = "q1";
                }
                else if (caracter == '*')
                {
                    estadoSiguiente = "q4";
                }
                break;

            case "q4":
                if (caracter == 'a' || caracter == '*')
                {
                    estadoSiguiente = "q4";
                }
                else if (caracter == '#')
                {
                    // La cadena es válida si termina en el estado q4 con el carácter '#'
                    esValida = true;
                    pilaAceptados.Push(caracter);  // Aceptar el '#' como parte de la cadena
                    estadoSiguiente = "q4";  // Mantener el estado como q4 (estado final)
                    break;
                }
                break;
        }

        // Agregar la transición a la tabla de transiciones general
        if (estadoSiguiente != null)
        {
            if (!tablaTransicionesGeneral.ContainsKey(estadoActual))
            {
                tablaTransicionesGeneral[estadoActual] = new Dictionary<char, string>();
            }

            if (!tablaTransicionesGeneral[estadoActual].ContainsKey(caracter))
            {
                tablaTransicionesGeneral[estadoActual][caracter] = estadoSiguiente;
            }

            // Actualizar el árbol de derivación y las pilas
            Node nextNode = new Node(estadoSiguiente);
            currentNode.Children.Add(nextNode);
            currentNode = nextNode;

            pilaAceptados.Push(caracter);  // Agregar a la pila de caracteres aceptados

            estadoActual = estadoSiguiente;
        }
        else
        {
            // Agregar a la pila de errores
            pilaErrores.Push(caracter);

            Node errorNode = new Node($"Error ({caracter})");
            currentNode.Children.Add(errorNode);
            currentNode = errorNode;
        }
    }

    return esValida;  // Devolver si la cadena fue válida o no
}
    static void PrintTransitionTable(Dictionary<string, Dictionary<char, string>> tablaTransiciones)
    {
        char[] caracteres = { 'a', 'b', '*', '#' };

        Console.WriteLine("      a     b     *     #");
        Console.WriteLine("    ------------------------");

        foreach (var estado in tablaTransiciones)
        {
            Console.Write($"{estado.Key}  ");

            foreach (var caracter in caracteres)
            {
                if (estado.Value.ContainsKey(caracter))
                {
                    Console.Write($"{estado.Value[caracter],-5} ");
                }
                else
                {
                    Console.Write($"{"-",-5} ");
                }
            }

            Console.WriteLine();
        }
    }

    static void PrintTree(Node node, string indent, bool last)
    {
        Console.Write(indent);
        if (last)
        {
            Console.Write("└─");
            indent += "  ";
        }
        else
        {
            Console.Write("├─");
            indent += "| ";
        }
        Console.WriteLine(node.Value);

        for (int i = 0; i < node.Children.Count; i++)
        {
            PrintTree(node.Children[i], indent, i == node.Children.Count - 1);
        }
    }
}


