using CG_Biblioteca;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gcgcg
{
    internal class Spline : Objeto
    {
        public IList<Ponto> pontos;

        private Ponto4D pontoInferiorEsquerdo;
        private Ponto4D pontoSuperiorEsquerdo;
        private Ponto4D pontoInferiorDireito;
        private Ponto4D pontoSuperiorDireito;

        private SegReta retaAB;
        private SegReta retaBC;
        private SegReta retaCD;

        public int indexPonto = 0;

        private int qtdPontosSpline = 10;

        public Spline(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = OpenTK.Graphics.OpenGL4.PrimitiveType.LineLoop;
            ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            PrimitivaTamanho = 20;

            pontoInferiorEsquerdo = new Ponto4D(-0.5, -0.5);
            pontoSuperiorEsquerdo = new Ponto4D(-0.5,  0.5);
            pontoInferiorDireito  = new Ponto4D( 0.5, -0.5);
            pontoSuperiorDireito  = new Ponto4D( 0.5,  0.5);

            pontos = [
                new Ponto(this, ref _rotulo, pontoInferiorDireito),
                new Ponto(this, ref _rotulo, pontoSuperiorDireito),
                new Ponto(this, ref _rotulo, pontoSuperiorEsquerdo),
                new Ponto(this, ref _rotulo, pontoInferiorEsquerdo)];

            retaAB = new SegReta(this, ref _rotulo, pontoInferiorDireito, pontoSuperiorDireito);
            retaAB.ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            
            retaBC = new SegReta(this, ref _rotulo, pontoSuperiorDireito, pontoSuperiorEsquerdo);
            retaBC.ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");

            retaCD = new SegReta(this, ref _rotulo, pontoSuperiorEsquerdo, pontoInferiorEsquerdo);
            retaCD.ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");

            Atualizar();
        }

        void CalcularPontosSpline()
        {
            base.pontosLista.Clear();

            for (var idxPonto = 1; idxPonto < qtdPontosSpline; idxPonto++)
            {
                var tempo = (1.0 / qtdPontosSpline) * idxPonto;

                var p1p2 = PontoIntermediario(pontoInferiorEsquerdo, pontoSuperiorEsquerdo, tempo);
                var p2p3 = PontoIntermediario(pontoSuperiorEsquerdo, pontoSuperiorDireito, tempo);
                var p3p4 = PontoIntermediario(pontoSuperiorDireito, pontoInferiorDireito, tempo);
            
                var p1p2p3 = PontoIntermediario(p1p2, p2p3, tempo);
                var p2p3p4 = PontoIntermediario(p2p3, p3p4, tempo);
            
                var p1p2p3p4 = PontoIntermediario(p1p2p3, p2p3p4, tempo);
                base.PontosAdicionar(p1p2p3p4);
            }

            base.PontosAdicionar(pontoInferiorDireito);
            base.PontosAdicionar(pontoSuperiorDireito);
            base.PontosAdicionar(pontoSuperiorEsquerdo);
            base.PontosAdicionar(pontoInferiorEsquerdo);

            base.ObjetoAtualizar();
        }

        Ponto4D PontoIntermediario(Ponto4D pontoA, Ponto4D pontoB, double tempo)
        {
            double X = pontoA.X + (pontoB.X - pontoA.X) * tempo;
            double Y = pontoA.Y + (pontoB.Y - pontoA.Y) * tempo;
            return new Ponto4D(X, Y);
        }

        public void SplineQtdPto(int inc)
        {
            if (qtdPontosSpline + inc <= 0)
            {
                qtdPontosSpline = 1;
                return;
            }

            if (qtdPontosSpline + inc > 10)
            {
                qtdPontosSpline = 10;
                return;
            }

            qtdPontosSpline += inc;
        }

        public void Atualizar()
        {
            int indexAnterior = 0;
            if (indexPonto == 0)
                indexAnterior = pontos.Count - 1;
            else
                indexAnterior = indexPonto - 1;

            pontos[indexAnterior].ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            pontos[indexPonto].ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");

            retaAB.ObjetoAtualizar();
            retaBC.ObjetoAtualizar();
            retaCD.ObjetoAtualizar();

            CalcularPontosSpline();
        }
    }
}
