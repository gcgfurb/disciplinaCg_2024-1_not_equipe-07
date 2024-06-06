using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace gcgcg
{
    internal class EditorVetorial : Objeto
    {
        public List<Poligono> poligonos = new List<Poligono>();

        public int idxPoliogonoSelecionado = -1;
        public int idxPoligonoAlterando = -1;
        
        public EditorVetorial(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
        }

        public Poligono GetPoligonoAtualSelecionado()
        {
            if (idxPoliogonoSelecionado == -1)
                return null;

            return poligonos[idxPoliogonoSelecionado];
        }

        public Poligono GetPoligonoAtualAlterando()
        {
            if (idxPoligonoAlterando == -1)
                return null;

            if (poligonos.Count == 0 || idxPoligonoAlterando > poligonos.Count - 1)
                return null;

            return poligonos[idxPoligonoAlterando];
        }

        public void AdicionarPoligono(Poligono _poligono)
        {
            poligonos.Add(_poligono);
            idxPoligonoAlterando = poligonos.Count - 1;
        }

        public void AlterarPontoPoligonoAtual(int idxPosicao, Ponto4D _ponto)
        {
            poligonos[idxPoligonoAlterando].PontosAlterar(_ponto, idxPosicao);
        }

        public void AdicionarPontoPoligonoAtual(Ponto4D _ponto)
        {
            poligonos[idxPoligonoAlterando].PontosAdicionar(_ponto);
        }

        public void MoverVerticeMaisProximoPoligono(Ponto4D pontoMouse)
        {
            if (idxPoliogonoSelecionado == -1)
                return;

            double menorDistancia = 0;
            int idxPontoMenorDistancia = -1;

            for (int idxPonto = 0; idxPonto < poligonos[idxPoliogonoSelecionado].PontosListaTamanho; ++idxPonto)
            {
                double distancia = Matematica.Distancia(pontoMouse, poligonos[idxPoliogonoSelecionado].PontosId(idxPonto));

                if (distancia < menorDistancia || idxPonto == 0)
                {
                    idxPontoMenorDistancia = idxPonto;
                    menorDistancia = distancia;
                }
            }

            if (idxPontoMenorDistancia != -1)
                poligonos[idxPoliogonoSelecionado].PontosAlterar(pontoMouse, idxPontoMenorDistancia);
        }

        public void RemoverVerticeMaisProximoPoligono(Ponto4D pontoMouse)
        {
            //if (GetPoligonoAtualSelecionado() == null)
            //    return;

            double menorDistancia = 0;
            int idxPontoMenorDistancia = -1;

            for (int idxPonto = 0; idxPonto < poligonos[idxPoliogonoSelecionado].PontosListaTamanho; ++idxPonto)
            {
                double distancia = Matematica.Distancia(pontoMouse, poligonos[idxPoliogonoSelecionado].PontosId(idxPonto));

                if (distancia < menorDistancia || idxPonto == 0)
                {
                    idxPontoMenorDistancia = idxPonto;
                    menorDistancia = distancia;
                }
            }

            if (idxPontoMenorDistancia != -1)
                poligonos[idxPoliogonoSelecionado].PontosRemover(idxPontoMenorDistancia);
        }

        public void RemoverPoligono()
        {
            if (idxPoliogonoSelecionado == -1)
                return;

            poligonos[idxPoliogonoSelecionado].PontosLimpar();
            poligonos.RemoveAt(idxPoliogonoSelecionado);

            idxPoliogonoSelecionado = -1;
            idxPoligonoAlterando = -1;

            base.ObjetoAtualizar();
        }

        public void AlteraAberturaFechamentoPoligonoAtual()
        {
            if (idxPoliogonoSelecionado == -1)
                return;

            poligonos[idxPoliogonoSelecionado].PrimitivaTipo = (poligonos[idxPoliogonoSelecionado].PrimitivaTipo == PrimitiveType.LineStrip) ? PrimitiveType.LineLoop : PrimitiveType.LineStrip;
        }

        public void SelecionaPoligono(Ponto4D pontoMouse)
        {
            // Se o número total de cruzamentos é ímpar, o ponto está dentro do polígono.

            int qtdPontosCruzados = 0;

            for (int idxPoligono = 0; idxPoligono < poligonos.Count; ++idxPoligono)
            {
                Poligono objPoligono = poligonos[idxPoligono];

                if (!objPoligono.Bbox().Dentro(pontoMouse))
                    continue;

                if (Matematica.ScanLine(pontoMouse, objPoligono.PontosId(0), objPoligono.PontosId(objPoligono.PontosListaTamanho - 1)))
                    ++qtdPontosCruzados;

                for (int idxPonto = 0; idxPonto < objPoligono.PontosListaTamanho - 1; ++idxPonto)
                {
                    Ponto4D ponto1 = objPoligono.PontosId(idxPonto);
                    Ponto4D ponto2 = objPoligono.PontosId(idxPonto + 1);

                    if (Matematica.ScanLine(pontoMouse, ponto1, ponto2))
                        ++qtdPontosCruzados;
                }


                if (qtdPontosCruzados % 2 != 0)
                {
                    idxPoliogonoSelecionado = idxPoligono;
                    return;
                }
            }

            idxPoliogonoSelecionado = -1;
        }
    }
}
