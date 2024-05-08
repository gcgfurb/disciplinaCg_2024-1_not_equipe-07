using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;

namespace gcgcg;

internal class Joystick
{
    private const double INCREMENTO_DESLOCAMENTO = 0.010;

    private Retangulo _retangulo;
    private Circulo _circuloMaior;
    private Ponto _ponto;
    private Circulo _circuloMenor;

    public Objeto ObjetoSelecionado => _ponto;

    public Joystick(Objeto paiRef, ref char rotulo)
    {
        double raio = 0.30;
        Ponto4D ptoDeslocamento = new(0.3, 0.3);

        Ponto4D pontoSupDir = Matematica.GerarPtosCirculo(45, raio);
        double deslocamento = pontoSupDir.X * -1;

        pontoSupDir.X += 0.3;
        pontoSupDir.Y += 0.3;

        Ponto4D pontoInfEsq = new()
        {
            X = 0.3 + deslocamento,
            Y = 0.3 + deslocamento
        };

        _retangulo = new Retangulo(paiRef, ref rotulo, pontoInfEsq, pontoSupDir);
        _circuloMaior = new Circulo(paiRef, ref rotulo, raio, ptoDeslocamento);
        _circuloMaior.PrimitivaTipo = PrimitiveType.LineLoop;
        _circuloMaior.PrimitivaTipo = PrimitiveType.LineLoop;

        _ponto = new Ponto(paiRef, ref rotulo, ptoDeslocamento)
        {
            PrimitivaTamanho = 10
        };

        _circuloMenor = new Circulo(paiRef, ref rotulo, 0.10, ptoDeslocamento);
        _circuloMenor.PrimitivaTipo = PrimitiveType.LineLoop;
    }

    public void MoveDireita()
    {
        Ponto4D novoPonto = new(_ponto.PontosId(0).X + INCREMENTO_DESLOCAMENTO, _ponto.PontosId(0).Y);
        Deslocar(novoPonto);
    }

    public void MoveEsquerda()
    {
        Ponto4D novoPonto = new(_ponto.PontosId(0).X - INCREMENTO_DESLOCAMENTO, _ponto.PontosId(0).Y);
        Deslocar(novoPonto);
    }

    public void MoveCima()
    {
        Ponto4D novoPonto = new(_ponto.PontosId(0).X, _ponto.PontosId(0).Y + INCREMENTO_DESLOCAMENTO);
        Deslocar(novoPonto);
    }

    public void MoveBaixo()
    {
        Ponto4D novoPonto = new(_ponto.PontosId(0).X, _ponto.PontosId(0).Y - INCREMENTO_DESLOCAMENTO);
        Deslocar(novoPonto);
    }

    private void Deslocar(Ponto4D novoPonto)
    {
        if (_retangulo.Bbox().Dentro(novoPonto))
        {
            _retangulo.PrimitivaTipo = PrimitiveType.LineLoop;
        }
        else
        {
            if (Matematica.distanciaQuadrado(novoPonto, _circuloMaior.PontoDeslocamento) > Math.Pow(_circuloMaior.raio, 2))
                return;

            _retangulo.PrimitivaTipo = PrimitiveType.Points;
        }

        _ponto.PontosAlterar(novoPonto, 0);
        _circuloMenor.Atualizar(novoPonto);
    }
}