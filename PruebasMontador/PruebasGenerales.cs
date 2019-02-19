using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MontadorDeUnidades;

namespace PruebasMontador
{
    [TestClass]
    public class PruebasGenerales
    {
        /// <summary>
        /// Montador de unidades
        /// </summary>
        private Montador _montador;

        /// <summary>
        /// Inicializador de pruebas
        /// </summary>
        [TestInitialize]
        public void Constructor()
        {
            _montador = new Montador();
        }

        /// <summary>
        /// Prueba una conexión exitosa a una unidad de red
        /// </summary>
        [TestMethod]
        public void PruebaConectarBueno()
        {
            var montadoCorrecto = _montador.Conectar("X", @"\\10.10.16.17\Compartida", "desarrollo", "D354rr0ll0");
            if (montadoCorrecto.Key)
            {
                Assert.IsTrue(Directory.Exists(@"X:\"));
            }
        }

        /// <summary>
        /// Prueba una desconcexión exitosa de una unidad de red
        /// </summary>
        [TestMethod]
        public void PruebaDesconectarBueno()
        {
            var desmontarCorrecto = _montador.Desconectar("X");
            Assert.IsTrue(desmontarCorrecto.Key);
        }

        /// <summary>
        /// Prueba una conexión fallida con datos incorrectos
        /// </summary>
        [TestMethod]
        public void PruebaConectarMalo()
        {
            var montadoCorrecto = _montador.Conectar("D", @"\\10.10.16.1\asdadf", "adfadfadf", "adfadfadf");
            Assert.IsFalse(montadoCorrecto.Key);
            Assert.IsTrue(montadoCorrecto.Value == "El dispositivo esta ocupado");
        }

        /// <summary>
        /// Prueba una desconexión fallida con un dispositivo montado en local
        /// </summary>
        [TestMethod]
        public void PruebaDesconectarMalo()
        {
            var desmontarCorrecto = _montador.Desconectar("D");
            Assert.IsFalse(desmontarCorrecto.Key);
            Assert.IsTrue(desmontarCorrecto.Value == "El dispositivo no es un dispositivo de red");
        }

        [TestMethod]
        public void PruebaDesconectarCarpetaEnUso()
        {
            var montadoCorrecto = _montador.Conectar("X", @"\\10.10.16.17\Compartida", "desarrollo", "D354rr0ll0");
            if (!montadoCorrecto.Key)
            {
                // Error
            }

            var respuestaDesconecta = _montador.Desconectar("X");
            Assert.IsTrue(respuestaDesconecta.Key);
        }
    }
}
