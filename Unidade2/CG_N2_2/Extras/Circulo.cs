using CG_Biblioteca;
using System;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public double raio = 0;
        double angulo = Convert.ToDouble(360 / 72);
        public Ponto4D PontoDeslocamento { get; private set; }

        public Circulo(Objeto _paiRef, ref char _rotulo, double _raio) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            raio = _raio;

            Atualizar(null);
        }

        public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            raio = _raio;

            Atualizar(ptoDeslocamento);
        }

        public void Atualizar(Ponto4D ptoDeslocamento)
        {
            base.pontosLista.Clear();

            PontoDeslocamento = ptoDeslocamento;

            double anguloDeslocado = 0;
            for (int idx = 0; idx < 72; ++idx)
            {
                anguloDeslocado += angulo;
                Ponto4D ponto = Matematica.GerarPtosCirculo(anguloDeslocado, raio);

                if (PontoDeslocamento != null)
                {
                    ponto.X += PontoDeslocamento.X;
                    ponto.Y += PontoDeslocamento.Y;
                }

                PontosAdicionar(ponto);
            }

            base.ObjetoAtualizar();
        }
    }
}
