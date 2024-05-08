using CG_Biblioteca;
using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;

namespace gcgcg
{
    internal class SrPalito : Objeto
    {
        double raio = 0;
        double angulo = 0;
        double deslocamento = 0;

        Ponto4D ptCabeca;
        Ponto4D ptPe;

        public SrPalito(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            raio = 0.5;

            ptCabeca = new Ponto4D(0.5, 0.5);
            ptPe = new Ponto4D(0.0, 0.0);

            base.PontosAdicionar(ptCabeca);
            base.PontosAdicionar(ptPe);

            AtualizarAngulo(45);
        }

        public void AtualizarPe(double peInc)
        {
            deslocamento += peInc;

            PontosId(0).X += peInc;
            PontosId(1).X += peInc;

            Atualizar();
        }

        public void AtualizarRaio(double raioInc)
        {
            raio += raioInc;

            AtualizarAngulo(0);
            Atualizar();
        }

        public void AtualizarAngulo(double anguloInc)
        {
            angulo += anguloInc;

            Ponto4D ponto = Matematica.GerarPtosCirculo(angulo, raio);
            ponto.X += deslocamento;

            PontosAlterar(ponto, 0);
        }

        private void Atualizar()
        {

            base.ObjetoAtualizar();
        }
    }
}
