using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace trabajo_camp6
{
    public partial class Form1 : Form
    {
        public class Cliente
        {
            // almacenar el registro de un préstamo
            public string DNI { get; set; }          // Crucial para la búsqueda en Pestaña 2
            public string Nombre { get; set; }       //  registro  de nombre 
            public decimal Monto { get; set; }       // Monto del préstamo 
            public int Periodo { get; set; }         // Período de tiempo (6, 12, 24 meses) 
            public decimal TasaInteres { get; set; } // Tasa de interés calculada 
            public DateTime FechaRegistro { get; set; } // Fecha de registro 

          
            public Cliente(string dni, string nombre, decimal monto, int periodo, decimal tasa, DateTime fecha)
            {
                DNI = dni;
                Nombre = nombre;
                Monto = monto;
                Periodo = periodo;
                TasaInteres = tasa;
                FechaRegistro = fecha;
            }
        }
        

        [cite_start]// Arreglo Unidimensional, puede ingresar asta 10 usuarios 
        private Cliente[] _registrosClientes = new Cliente[10];

        // Contador para gestionar la posición actual del arreglo
        private int _contadorClientes = 0;

       [cite_start]// Arreglo Bidimensional: Almacena las tasas de interés 
                    // Filas: 0=Baja, 1=Media, 2=Alta
                    // Columnas: 0=6 meses, 1=12 meses, 2=24 meses
        private decimal[,] _tasasInteres = new decimal[,]
        {
    // 6 meses, 12 meses, 24 meses
    { 0.05m, 0.08m, 0.12m }, // Baja
    { 0.10m, 0.15m, 0.20m }, // Media
    { 0.18m, 0.25m, 0.35m }  // Alta
        };
        private object lstCronograma;

        public Form1()
        {
            InitializeComponent();
        }
       [cite_start]// Método con retorno para calcular la tasa de interés
        private decimal CalcularTasaInteres(int periodo, string categoria)
        {
            int fila;    // Índice de categoría (Baja, Media, Alta)
            int columna; // Índice de período (6, 12, 24)

            // Determinar Fila (Categoría)
            if (categoria == "Baja") fila = 0;
            else if (categoria == "Media") fila = 1;
            else if (categoria == "Alta") fila = 2;
            else return 0m;

            // Determinar Columna (Período)
            if (periodo == 6) columna = 0;
            else if (periodo == 12) columna = 1;
            else if (periodo == 24) columna = 2;
            else return 0m;

            // Retornar el valor del arreglo bidimensional
            return _tasasInteres[fila, columna];
        }

        [cite_start]// Método con retorno para buscar el cliente por DNI 
        private Cliente BuscarClientePorDNI(string dni)
        {
            // Iterar sobre los clientes registrados
            for (int i = 0; i < _contadorClientes; i++)
            {
                [cite_start] if (_registrosClientes[i].DNI == dni) // Se asume que el DNI es único
                {
                    return _registrosClientes[i]; // Retorna el objeto Cliente encontrado
                }
            }
            return null; // Retorna null si no se encuentra
        }

        [cite_start]// Método sin retorno de valores para generar el cronograma de pagos 
        private void GenerarCronograma(Cliente clienteEncontrado)
        {
            lstCronograma1.Items.Clear(); // Limpiar el ListBox

            // Si la fórmula es simple (Monto + Intereses) / Meses:
            decimal montoTotalConInteres = clienteEncontrado.Monto * (1 + clienteEncontrado.TasaInteres);
            decimal montoMensual = montoTotalConInteres / clienteEncontrado.Periodo; // Monto de cada pago mensual 

            [cite_start]// Mostrar el resumen del préstamo 
            lblTotalPagar.Text = $"Tasa: {clienteEncontrado.TasaInteres:P0} | Total a Pagar: {montoTotalConInteres:C}";

            // Generar las entradas del cronograma
            for (int mes = 1; mes <= clienteEncontrado.Periodo; mes++)
            {
                [cite_start]// Agregar la información de pago al ListBox 
               int v = lstCronograma1.Items.Add($"Mes {mes}: {montoMensual:C}");
                
            }
        }

        private void OrdenarClientesPorMontoAscendente()
        {
            
            for (int i = 0; i < _contadorClientes - 1; i++)
            {
                for (int j = 0; j < _contadorClientes - i - 1; j++)
                {
                    // Si el monto actual es mayor que el siguiente, intercámbialos
                    if (_registrosClientes[j].Monto > _registrosClientes[j + 1].Monto)
                    {
                        // Realizar el intercambio de objetos
                        Cliente temp = _registrosClientes[j];
                        _registrosClientes[j] = _registrosClientes[j + 1];
                        _registrosClientes[j + 1] = temp;
                    }
                }
            }
            lblMensajes.Text = "Clientes ordenados por Monto (Ascendente).";
            
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            // 1. Validar datos
            if (!ValidarRegistro())
            {
                return;
            }

            // 2. Verificar capacidad del arreglo
            if (_contadorClientes >= _registrosClientes.Length)
            {
                lblMensajes.Text = "Error: Límite de registros alcanzado (Máx 10).";
                return;
            }

            // 3. Recolectar y calcular datos
            string dni = txtDNI.Text;
            string nombre = txtNombre.Text;
            decimal monto = decimal.Parse(txtMonto.Text);
            int periodo = int.Parse(cmbPeriodo.SelectedItem.ToString());
            DateTime fecha = dtpFechaRegistro.Value;

            string categoria = rbBaja.Checked ? "Baja" :
                               rbMedia.Checked ? "Media" : "Alta";

            decimal tasaCalculada = CalcularTasaInteres(periodo, categoria);

            // 4. Crear y almacenar el nuevo Cliente
            Cliente nuevoCliente = new Cliente(dni, nombre, monto, periodo, tasaCalculada, fecha);

            _registrosClientes[_contadorClientes] = nuevoCliente; // Almacenar en el arreglo
            _contadorClientes++;

            lblMensajes.Text = $"Registro Exitoso para {nombre}. Tasa Aplicada: {tasaCalculada:P0}";
        }

        private bool ValidarRegistro()
        {
            throw new NotImplementedException();
        }

            private void btnBuscar_Click(object sender, EventArgs e)
        {
            string dniBuscado = txtBuscarDNI.Text;

            // 1. Buscar el cliente
            Cliente cliente = BuscarClientePorDNI(dniBuscado); // Usa el método con retorno

            if (cliente != null)
            {
                // 2. Generar y mostrar el cronograma
                GenerarCronograma(cliente); // Usa el método sin retorno 
            }
            else
            {
                // Mostrar error si no se encuentra
                lblTotalPagar.Text = "Error: Cliente no encontrado con ese DNI.";
                lstCronograma1.Items.Clear();
            }
        }
    }
    }
}
