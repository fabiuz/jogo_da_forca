using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

namespace Jogo_da_Forca
{
    struct forca_palavras
    {
        public string strPalavra;
        public List<string> strDicas;
        public bool bPalavra_Ja_Sorteada;
    }

    // Esta estrutura guardará as posições que serão apagadas, do boneco.
    struct point
    {
        public int x, y;

        point(int iX, int iY)
        {
            x = iX;
            y = iY;
        }
    }


    class Jogo_da_Forca
    {
        private int[,] boneco;

        // O boneco é dividido em 13 partes.
        private const int BONECO_PARTES = 13;

        // Toda vez que o usuário erra, esta variável é incrementada.
        // E parte do boneco é apagada.
        private int iBoneco_Parte_Corpo;

        // Indica uma lista de coordenadas, x e y, a apagar do boneco.
        private List<point> lista_corpo_x_y;

        // Cor de fundo e da letra
        // Muito mais simples, alterarmos em um único lugar.
        private const ConsoleColor COR_DE_FUNDO = ConsoleColor.Blue;
        private const ConsoleColor COR_DA_LETRA = ConsoleColor.White;

        // Toda vez que o usuário realizar um movimento nas teclas de direção está variável
        // será definida para true, e também, quando executarmos o jogo pela primeira vez ou
        // reiniciarmos o jogo, bAtualizar será true.
        private bool bAtualizar;
 
        // Toda vez quem uma palavra é sorteada, devemos marcar
        // true na posição especificada.
        private bool[] bPalavra_Ja_Sorteada;

        // Cada posição do arranjo abaixo será definido true, se o usuário acertou o caractere
        // naquela posição.
        bool[] bPalavra_Encontrada;

        // Toda vez que o usuário achar todos os caracteres, esta variável é definida pra true.
        // Para que seja sorteada uma nova palavra.
        bool bProxima_Palavra;
        bool bTodas_Palavras_Lidas;

        // Indica a posição da próxima dica, se existir.
        private int iDica_Indice;

        // Vamos manter uma variável string para guardar a última dica
        // Isto serve para que não ocorra lixo na tela.
        private string strDica_Anterior;

        // Indica qual palavra utilizar.
        private int iPalavra_Indice;
        

        // Toda vez que um caractere é localizado, é incrementado um ponto
        private int iPonto;

        // Toda vez que o usuário erra um caractere é indicado um erro.
        private int iErro;

        private bool bPerdeu_Jogo;

        // Esta variável será true, se:
        // O usuário abriu o jogo da forca, para jogar pela primeira vez.
        // Estava jogando um jogo e resolveu reiniciar o jogo.
        // O usuário acertou todas as palavras e quiz jogar novamente.
        // O usuário perdeu e quiz jogar novamente.
        private bool bNovo_Jogo;

        // O usuário quer abandonar o jogo e reiniciar um novo jogo.
        private bool bReiniciar_Jogo;

        // Se o usuário pressiona 'ESC' enquanto joga, esta variável será definida pra true
        // Mas antes será perguntado se o usuário deseja abandonar o jogo.
        private bool bSair_do_Jogo;

        // Toda vez que um novo jogo é inicializado ou uma nova palavra é sorteada
        // tempo_Inicial guardará o tempo que começou o jogo ou a nova palavra foi sorteada.
        DateTime tempo_Inicial, tempo_Final;

        // Esta variável é a principal do jogo
        // Após abrirmos o arquivo de palavras e dicas
        // Tais informações estarão no arranjo abaixo.
        // Cada struct individual do arranjo abaixo, corresponde a uma palavra.
        // Dentro de cada struct individual estará uma lista de dicas, se houver, para a palavra correspondente.
        forca_palavras[] todas_palavras;
        private const long TEMPO_EM_SEGUNDOS_POR_PALAVRA = 30;

        Jogo_da_Forca()
        {

        }

        // Esta função cria o boneco que será utilizado no jogo da forca.
        // Cada parte do boneco é identificado por um número:
        // 1  - Pé esquerdo.
        // 2  - Pé direito.
        // 3  - Perna esquerda.
        // 4  - Perna direita.
        // 5  - Braço esquerdo.
        // 6  - Braço Direito.
        // 7  - Tronco.
        // 8  - Olho esquerdo.
        // 9  - Olho direito.
        // 10 - Nariz.
        // 11 - Boca.
        // 12 - Pescoço.
        // 13 - Cabeça.

        private const int BONECO_LINHAS = 21;
        private const int BONECO_COLUNAS = 11;

        private void Criar_Boneco()
        {
            boneco = new int[BONECO_LINHAS, BONECO_COLUNAS]
            {
                {  0,  0,  0, 13, 13, 13, 13, 13,  0,  0,  0, },  // [0] 
                {  0,  0, 13,  0,  0,  0,  0,  0, 13,  0,  0, },  // [1] 
                {  0, 13,  0,  0,  0,  0,  0,  0,  0, 13,  0, },  // [2] 
                {  0, 13,  0,  7,  0,  0,  0,  8,  0, 13,  0, },  // [3] 
                {  0, 13,  0,  0,  0,  9,  0,  0,  0, 13,  0, },  // [4] 
                {  0, 13,  0,  0,  0,  0,  0,  0,  0, 13,  0, },  // [5] 
                {  0, 13,  0,  0, 10, 10, 10,  0,  0, 13,  0, },  // [6] 
                {  0,  0, 13,  0,  0,  0,  0,  0, 13,  0,  0, },  // [7] 
                {  0,  0,  0, 13, 13, 13, 13, 13,  0,  0,  0, },  // [8] 
                {  0,  0,  0,  0, 13, 13, 13,  0,  0,  0,  0, },  // [9] 
                {  0,  5, 12, 12, 12, 12, 12, 12, 12,  6,  0, },  // [10] 
                {  5,  5, 12, 12, 12, 12, 12, 12, 12,  6,  6, },  // [11] 
                {  5,  0, 11, 11, 11, 11, 11, 11, 11,  0,  6, },  // [12] 
                {  5,  0, 11, 11, 11, 11, 11, 11, 11,  0,  6, },  // [13] 
                {  5,  0, 11, 11, 11, 11, 11, 11, 11,  0,  6, },  // [14] 
                {  5,  0,  0, 11, 11, 11, 11, 11,  0,  0,  6, },  // [15] 
                {  5,  0,  4,  4,  0,  0,  0,  3,  3,  0,  6, },  // [16] 
                {  0,  0,  4,  0,  0,  0,  0,  0,  3,  0,  0, },  // [17] 
                {  0,  0,  4,  0,  0,  0,  0,  0,  3,  0,  0, },  // [18] 
                {  0,  0,  4,  0,  0,  0,  0,  0,  3,  0,  0, },  // [19] 
                {  0,  1,  1,  1,  0,  0,  0,  2,  2,  2,  0, },  // [20] 
            };
            
            // Sempre devemos resetar onde a próxima parte do corpo será apagada.
            iBoneco_Parte_Corpo = 0;
        }

        // Aqui, estamos mapeando onde fica cada item na linha, cada item é baseado com o ítem 
        // anterior

        private const int BORDA_JANELA_LINHA_SUPERIOR = 0;
        private const int BORDA_JANELA_LINHA_INFERIOR = DICA_LINHA_INFERIOR + 1;

        private const int PONTUACAO_LINHA_SUPERIOR = BORDA_JANELA_LINHA_SUPERIOR + 1;
        private const int PONTUACAO_LINHA_INFERIOR = PONTUACAO_LINHA_SUPERIOR + 2;

        private const int BONECO_ESQUERDA = (80 - BONECO_COLUNAS) / 2;
        private const int BONECO_TOPO = PONTUACAO_LINHA_INFERIOR + 2;
        private const int BONECO_INFERIOR = BONECO_TOPO + BONECO_LINHAS - 1;

        // Onde fica a linha, há somente uma linha, então devemos considerar
        // PALAVRA_LINHA_SUPERIOR E PALAVRA_LINHA_INFERIOR iguais.
        private const int PALAVRA_LINHA_SUPERIOR = BONECO_INFERIOR + 2;
        private const int PALAVRA_LINHA_INFERIOR = PALAVRA_LINHA_SUPERIOR + 1;

        private const int DICA_LINHA_SUPERIOR = PALAVRA_LINHA_INFERIOR + 1;
        private const int DICA_LINHA_TEXTO = DICA_LINHA_SUPERIOR + 1;
        private const int DICA_LINHA_INFERIOR = DICA_LINHA_TEXTO + 1;

        // Toda vez que o boneco é atualizado, devemos redesenhar o boneco.
        // bDesenhar, será falso, quando precisarmos apagar o boneco desatualizado.
        private void Desenhar_Boneco(bool bDesenhar)
        {
            // Para centralizar horizontalmente o boneco, devemos subtrair 
            // a quantidade de colunas da janela menos a quantidade de colunas
            // que o boneco ocupa e dividir a diferença por 2.

            if (bDesenhar)
            {
                // 
                Console.BackgroundColor = COR_DE_FUNDO;
                Console.ForegroundColor = COR_DA_LETRA;

                // Aqui o fundo será branco.
                Console.ForegroundColor = COR_DA_LETRA;
                Console.BackgroundColor = COR_DA_LETRA;

                for (int iY = 0; iY < BONECO_LINHAS; iY++)
                {
                    for (int iX = 0; iX < BONECO_COLUNAS; iX++)
                    {
                        Console.CursorVisible = false;
                        Console.SetCursorPosition(BONECO_ESQUERDA + iX, BONECO_TOPO + iY);

                        if (bDesenhar)
                        {
                            if (boneco[iY, iX] != 0)
                            {
                                Console.Write((char)cFACE_SORRINDO);
                            }
                        }
                    }
                }

            }
            else
            {
                Console.BackgroundColor = COR_DE_FUNDO;
                Console.ForegroundColor = COR_DE_FUNDO;

                foreach (var coordenada in lista_corpo_x_y)
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(BONECO_ESQUERDA + coordenada.x, BONECO_TOPO + coordenada.y);
                    Console.Write((char)'@');
                }
                
            }
            Console.BackgroundColor = COR_DE_FUNDO;
            Console.ForegroundColor = COR_DA_LETRA;
            return;

        }
        
        // Cada vez que esta função é chamada quer dizer que o usuário digitou algum
        // caractere que não está na palavra.
        private void Atualizar_Boneco()
        {
            // Cada vez que é chamada a próxima parte do corpo do boneco é apagada.
            iBoneco_Parte_Corpo++;
            iErro++;

            // Se as trezes partes foram apagadas, indicar que perdeu.
            if(iBoneco_Parte_Corpo == BONECO_PARTES)
            {
                bPerdeu_Jogo = true;
            }

            lista_corpo_x_y = new List<point>();

            for (int iY = 0; iY < BONECO_LINHAS; iY++)
            {
                for (int iX = 0; iX < BONECO_COLUNAS; iX++)
                {
                    if (boneco[iY, iX] == iBoneco_Parte_Corpo)
                    {
                        boneco[iY, iX] = 0;
                        lista_corpo_x_y.Add(new point() { x = iX, y = iY });
                    }
                }
            }

            Desenhar_Boneco(false);
        }

        private const int iTELA_INICIAL_LINHAS = 25;
        private const int iTELA_INICIAL_COLUNAS = 28;

        private void Tela_Inicial()
        {
            bool[,] tela = new bool[iTELA_INICIAL_LINHAS, iTELA_INICIAL_COLUNAS]
            {
                {  false,  false,  false,  true ,  false,  false,  false,  false,  true ,  true ,  true ,  false,  false,  false,  false,  false,  true ,  true ,  true ,  true ,  false,  false,  false,  false,  true ,  true ,  true ,  false, },  // [0] 
                {  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [1] 
                {  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [2] 
                {  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [3] 
                {  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [4] 
                {  true ,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [5] 
                {  true ,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [6] 
                {  false,  true ,  true ,  false,  false,  false,  false,  false,  true ,  true ,  true ,  false,  false,  false,  false,  false,  true ,  true ,  true ,  false,  false,  false,  false,  false,  true ,  true ,  true ,  false, },  // [7] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [8] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  false,  true ,  true ,  false,  false,  false,  false,  true ,  true ,  true ,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [9] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  true ,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [10] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [11] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  true ,  true ,  true ,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [12] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [13] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  true ,  false,  false,  true ,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [14] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  false,  true ,  true ,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [15] 
                {  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false,  false, },  // [16] 
                {  false,  true ,  true ,  true ,  false,  false,  true ,  true ,  true ,  false,  false,  false,  true ,  true ,  true ,  false,  false,  false,  true ,  true ,  true ,  false,  false,  false,  true ,  true ,  true ,  false, },  // [17] 
                {  true ,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true , },  // [18] 
                {  true ,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [19] 
                {  false,  true ,  true ,  true ,  false,  true ,  false,  false,  false,  true ,  false,  false,  true ,  true ,  true ,  false,  false,  true ,  false,  false,  false,  false,  false,  false,  true ,  true ,  true ,  false, },  // [20] 
                {  true ,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [21] 
                {  true ,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  true ,  false,  false,  true ,  false,  false,  false,  false,  false,  true ,  false,  false,  false,  true , },  // [22] 
                {  true ,  false,  false,  false,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true ,  false,  true ,  false,  false,  false,  true , },  // [23] 
                {  true ,  false,  false,  false,  false,  false,  true ,  true ,  true ,  false,  false,  true ,  false,  false,  false,  true ,  false,  false,  true ,  true ,  true ,  false,  false,  true ,  false,  false,  false,  true , },  // [24] 
            };
            

            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DE_FUNDO;
            Console.Clear();

            Desenhar_Borda_da_Janela("");


            // Aqui o fundo será bra
            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DA_LETRA;

            // Vamos centralizar horizontalmente a tela inicial do jogo.
            int iTela_Inicial_Esquerda = (80 - iTELA_INICIAL_COLUNAS) / 2;
            int iTela_Inicial_Topo = (BORDA_JANELA_LINHA_INFERIOR  + 1 - iTELA_INICIAL_LINHAS) / 2;

            for (int iY = 0; iY < iTELA_INICIAL_LINHAS; iY++)
            {
                for (int iX = 0; iX < iTELA_INICIAL_COLUNAS; iX++)
                {

                    Console.CursorVisible = false;
                    Console.SetCursorPosition(iTela_Inicial_Esquerda + iX, iTela_Inicial_Topo + iY);

                    if (tela[iY, iX])
                    {
                        Console.Write((char)'#');
                    }
                }
            }

            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DA_LETRA;
            Console.SetCursorPosition(1, BORDA_JANELA_LINHA_INFERIOR + 1);
            Console.Write(new string(' ', 15));

            Console.ForegroundColor = COR_DE_FUNDO;
            Console.BackgroundColor = COR_DA_LETRA;
            Console.SetCursorPosition(1, BORDA_JANELA_LINHA_INFERIOR + 1);
            Console.Write(" F1 - JOGAR ");

            int iEsc_Direita = 80 - 15 - 1;
            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DA_LETRA;
            Console.SetCursorPosition(iEsc_Direita, BORDA_JANELA_LINHA_INFERIOR + 1);
            Console.Write(new string(' ', 15));

            Console.ForegroundColor = COR_DE_FUNDO;
            Console.BackgroundColor = COR_DA_LETRA;
            Console.SetCursorPosition(iEsc_Direita, BORDA_JANELA_LINHA_INFERIOR + 1);
            Console.Write(" ESC - SAIR ");


            // O jogo sempre começa com fundo azul e letra cor branca.
            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DE_FUNDO;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    // Vamos capturar a tecla que foi pressionada.
                    // Colocamos true, pois não queremos que a tecla apareça na tela.
                    ConsoleKeyInfo tecla_pressionada = Console.ReadKey(true);

                    switch (tecla_pressionada.Key)
                    {
                        case ConsoleKey.F1:
                            // Se o usuário pressionar F1, iniciar jogo.
                            //bJogar = true;
                            Console.Clear();
                            return;

                        case ConsoleKey.Escape:
                            bSair_do_Jogo = true;
                            Exibir_Tela_Pergunta();
                            if (bSair_do_Jogo)
                                return;
                            return;

                    }



                }


            }
        }

        public bool Carregar_Lista_de_Palavras(out string strErro)
        {
            // Vamos verificar se o arquivo existe.
            if (!File.Exists("jogo.txt"))
            {
                strErro = "Arquivo não existe.";
                return false;
            }

            // O arquivo deve está no formato utf-8, principalmente, se houver acento.
            string[] strLinhas = File.ReadAllLines("jogo.txt", Encoding.UTF8);

            // Se não há nada, informar com erro.
            if(strLinhas.Length == 0)
            {
                strErro = "Arquivo está vazio.";
                return false;
            }

            // Vamos utilizar uma consulta Linq, para saber quantas palavras começam com 'P:'
            // A pesquisa é insensível a maiúscula e minúscula.
            var strPalavras = from strLinha in strLinhas
                             where strLinha.ToLower().StartsWith("p:")
                             select strLinha;

            // Aqui, iremos contar quantas palavras começa com 'P:'
            int iPalavras_Quantidade = strPalavras.Count();
            if(iPalavras_Quantidade== 0)
            {
                strErro = "Não há nenhuma palavra secreta.";
                return false;
            }

            // Conforme especificado pelo cliente, não pode haver mais de 100 palavras.
            if(iPalavras_Quantidade > 100)
            {
                strErro = "Há mais de 100 palavras, no arquivo jogo.txt";
                return false;
            }

            todas_palavras = new forca_palavras[iPalavras_Quantidade];
            bPalavra_Ja_Sorteada = new bool[iPalavras_Quantidade];

            // A primeira linha do arquivo sempre tem que começar com a palavra 'P:'
            if (!strLinhas[0].ToLower().StartsWith("p:"))
            {
                strErro = "A primeira linha do arquivo 'jogo.txt' não começa com 'P:'";
                return false;
            }

            // Cada vez que encontrarmos uma linha começando com 'p:' iremos incrementar estava variável.
            int iPalavra = -1;
            int iDicas_Quantidade = 0;

            for(int iA = 0; iA < strLinhas.Length; iA++)
            {
                // Cada linha deve começar com 'P:' ou 'D:', senão iremos indicar um erro ao usuário
                if (!strLinhas[iA].ToLower().StartsWith("p:") &&
                   !strLinhas[iA].ToLower().StartsWith("d:"))
                {
                    // Não começa com 'p:', nem com 'd:'
                    strErro = string.Format("Linha {0} não começa com 'P:' e nem com 'D:'", iA + 1);
                    return false;
                }

                // Quando encontrarmos uma linha começando com 'p:', devemos incrementar iPalavra
                // Para podermos manter trilha onde iremos colocar as próximas dicas
                // E também, inserir a palavra encontrada na 'lista_de_palavras_secretas'.
                if (strLinhas[iA].ToLower().StartsWith("p:"))
                {
                    iPalavra++;

                    // Toda vez que encontrarmos uma nova palavra, devemos zerar a quantidade de dicas.
                    iDicas_Quantidade = 0;

                    // A condição if abaixo, não é necessária pois já fizermos a consulta de quantos ítens 
                    // que começa com 'p:'
                    // Pois se iPalavra for igual ou maior que iPalavras_Quantidade, poderemos ter 
                    // uma exceção de erro de faixa no arranjo.
                    if(iPalavra == iPalavras_Quantidade)
                    {
                        strErro = "Erro não esperado, a quantidade de palavras é maior que o arranjo criado.";
                        return false;
                    }


                    // A palavra começa sempre após 'p:'
                    // Após retornarmos o string, devemos retirar os espaços iniciais e finais.
                    string strPalavra = strLinhas[iA].Substring(2).Trim();
                    
                    // A palavra não pode ser um string vazio, se for iremos considerar como um erro.
                    if(strPalavra == "")
                    {
                        strErro = "Palavra após 'P:' é um string vazio.";
                        return false;
                    }

                    // Toda vez que uma palavra é encontrada, é criada uma lista para guardar as dicas.
                    todas_palavras[iPalavra].strPalavra = strPalavra;
                    todas_palavras[iPalavra].strDicas = new List<string>();
                    todas_palavras[iPalavra].bPalavra_Ja_Sorteada = false;
                }

                // Se é uma dica, devemos colocar esta dica na variável 'lista_de_dicas'
                // 'lista_de_dicas' é uma arranjo de lista, cada lista corresponde a zero ou mais dicas
                // para uma palavra correspondente.
                // A variável iPalavra acima mantém trilha, onde cada dica deve ficar armazenada
                // na lista de dicas que corresponde a sua palavra.
                if (strLinhas[iA].ToLower().StartsWith("d:"))
                {

                    // Conforme especificação não pode haver mais de 10 dicas para uma palavra.
                    iDicas_Quantidade++;
                    if(iDicas_Quantidade > 10)
                    {
                        strErro = string.Format("Linha {0}, há mais de 10 dicas para a palavra: {1}",
                            iA, todas_palavras[iPalavra].strPalavra);
                        return false;
                    }

                    // Observe, que no layout do arquivo uma ou mais dicas, sempre vem após a palavra
                    // correspondente a qual está associada.
                    // iPalavra, mantém trilha, da última palavra encontrada.

                    // Devemos pegar somente a dica, que está após 'd:'
                    string strDica = strLinhas[iA].Substring(2).Trim();

                    // Se strDica está vazio, não iremos inserir, nem iremos considerar que é um erro.
                    if(strDica != "")
                    {
                        todas_palavras[iPalavra].strDicas.Add(strDica);
                    }

                }
            }

            strErro = "";
            return true;                       
        }

        // Retorna o índice da próxima palavra, que ainda não foi sorteada
        // Retorna -1, se não há mais palavras

        private int sortear_palavra()
        {
            // Vamos verificar se há ainda bolas a ser sorteada.
            var iQuantidade = (from nao_sorteada in bPalavra_Ja_Sorteada
                               where nao_sorteada == false
                               select nao_sorteada).Count();

            if (iQuantidade == 0)
            {
                return -1;
            }

            // Vamos sortear um número.
            Random numero_aleatorio = new Random();

            while (true)
            {
                int iNumero_Aleatorio = numero_aleatorio.Next(bPalavra_Ja_Sorteada.Length);
                if (!bPalavra_Ja_Sorteada[iNumero_Aleatorio])
                {
                    bPalavra_Ja_Sorteada[iNumero_Aleatorio] = true;
                    return iNumero_Aleatorio;
                }
            }

        }

        // Esta função será chamada, quando:
        // O usuário abriu o jogo da forca, para jogar pela primeira vez.
        // Estava jogando um jogo e resolveu reiniciar o jogo.
        // O usuário acertou todas as palavras e quiz jogar novamente.
        // O usuário perdeu e quiz jogar novamente.

        // Sempre que iniciar um novo jogo, devemos resetar o tempo e resetar o sorteio das palavrs.
        public void Novo_Jogo()
        {
            iPonto = 0;
            iErro = 0;

            Resetar_Tempo();
            Resetar_Sorteio_Palavra();

            bNovo_Jogo = false;
            bSair_do_Jogo = false;
            bProxima_Palavra = false;
        }

        // Toda vez que a pontuação é alterada ou usuário erra
        // Esta função é chamada.
        private void Exibir_Pontuacao()
        {

            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            // Exibe pontuação.
            // Fundo vermelho, letras em amarelo.
            Console.BackgroundColor = COR_DE_FUNDO;
            Console.ForegroundColor = COR_DA_LETRA;

            Desenhar_Borda_Simples(" PONTOS ", iPonto.ToString("0000000000"), 2, PONTUACAO_LINHA_SUPERIOR , 0);

            // A palavra 'ERROS: 0000000000', tem 16 caracteres
            // Há 80 colunas, de 0 a 79, então, 80 - 10 = 70.
            // Os números subtraídos são:
            // 10, o número de erros é compostos de 10 caracteres.
            // 2,  borda esquerda e direita ao redor da palavra.
            // 1,  borda direita da janela.
            int iErro_Posicao_X = 79 - 10 - 2 - 1;
            Desenhar_Borda_Simples(" ERROS ", iErro.ToString("0000000000"), iErro_Posicao_X, PONTUACAO_LINHA_SUPERIOR, 0);


        }
        

        // Esta função serve para atualizar os pontos obtidos e
        // exibir o cronômetro.
        private void Atualizar_Cronometro()
        {
            // Vamos contar os ticks.
            long ticks_decorridos = tempo_Final.Ticks - DateTime.Now.Ticks;
            TimeSpan tempo_restante = new TimeSpan(ticks_decorridos);
            
            Console.CursorVisible = false;

            // Como o título é maior que o tempo_restante, então iremos considerar
            // a quantidade de caracteres da palavra " TEMPO "
            int iCronometro_Esquerda = (80 - " TEMPO ".Length - 4) / 2;



            // Se faltam dez segundos para terminar, informa visualmente ao usuário.
            // Letra de cor vermelha.
            if (tempo_restante.TotalSeconds <= 10)
            {
                Console.BackgroundColor = COR_DE_FUNDO;
                Console.ForegroundColor = ConsoleColor.Red;
                Desenhar_Borda_Dupla(" TEMPO ", tempo_restante.TotalSeconds.ToString("00"), iCronometro_Esquerda, PONTUACAO_LINHA_SUPERIOR, 1);

            }
            else
            {
                Console.BackgroundColor = COR_DE_FUNDO;
                Console.ForegroundColor = COR_DA_LETRA;
                Desenhar_Borda_Simples(" TEMPO ", tempo_restante.TotalSeconds.ToString("00"), iCronometro_Esquerda, PONTUACAO_LINHA_SUPERIOR, 1);
            }



            if (tempo_restante.TotalSeconds <= 0)
            {
                bPerdeu_Jogo = true;
            }

        }

        
        // Toda vez que um novo jogo iniciar ou uma nova palavra for sorteada
        // O tempo será resetado
        private void Resetar_Tempo()
        {
            tempo_Inicial = DateTime.Now;
            tempo_Final = tempo_Inicial.AddSeconds(TEMPO_EM_SEGUNDOS_POR_PALAVRA);
        }

        // Toda vez que iniciar um novo jogo, resetar sorteio das palavras
        // A variável bPalavra_Ja_Sorteada mantêm trilha das palavras que já foram sorteadas
        // O valor do índice específico será true, se já foi sorteado.
        private void Resetar_Sorteio_Palavra()
        {
            for (int iA = 0; iA < bPalavra_Ja_Sorteada.Length; iA++)
                bPalavra_Ja_Sorteada[iA] = false;
        }

        private void Proxima_Palavra()
        {
            iPalavra_Indice = sortear_palavra();

            // Se retorna -1, quer dizer que não há mais palavras.
            bTodas_Palavras_Lidas = false;
            if(iPalavra_Indice == -1)
            {
                bTodas_Palavras_Lidas = true;
                return;
            }

            Resetar_Tempo();

            bPalavra_Encontrada = new bool[todas_palavras[iPalavra_Indice].strPalavra.Length];
            iDica_Indice = 0;

            bAtualizar = true;

            Criar_Boneco();
            Desenhar_Boneco(true);
            Resetar_Tempo();
        }




        public void Jogar()
        {
            string strErro = "";
            
            // O arquivo é carregado, uma única vez.
            if(Carregar_Lista_de_Palavras(out strErro)==false){
                Console.WriteLine("Erro: " + strErro);
                return;
            }

            // Ajustar janela de console para o jogo.
            Ajustar_Console_Para_Jogo();

            // O jogo sempre começa com fundo azul e letra cor branca.
            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DE_FUNDO;
            Console.Clear();

           Tela_Inicial();

            bNovo_Jogo = true;
            while (true)
            {
                while (true)
                {
                    if (bSair_do_Jogo)
                    {
                        Exibir_Tela_Pergunta();

                        if (bSair_do_Jogo)
                        {
                            return;
                        }
                    }

                    DateTime tempoAnterior = DateTime.Now;
                    while (true)
                    {

                        if (bNovo_Jogo)
                        {
                            bNovo_Jogo = false;
                            Novo_Jogo();

                            Proxima_Palavra();
                            
                            // Aqui, como estamos começando um novo jogo, todas as palavras não foram lidas.                            
                            if (bTodas_Palavras_Lidas)
                            {
                                throw new Exception("Erro gravíssimo, deve haver pelo menos uma palavra");
                                // return;
                            }

                            bSair_do_Jogo = false;
                            bAtualizar = true;
                            bProxima_Palavra = false;
                        }

                        // Se o usuário pressionou as teclas de direção, devemos atualizar a tela.
                        if (bAtualizar)
                        {
                            Desenhar_Borda_da_Janela(" JOGO DA FORCA ");
                            Exibir_Pontuacao();
                            Exibir_Palavra(true);
                            Atualizar_Dica();
                            Desenhar_Boneco(true);
                            Exibir_Rodape_Informativo();
                        }

                        // Vamos atualizar o cronômetro a cada 3 segundos.

                        long ticks_decorridos = DateTime.Now.Ticks - tempoAnterior.Ticks;
                        TimeSpan intervalo_tempo = new TimeSpan(ticks_decorridos);

                        if(intervalo_tempo.TotalSeconds >= 1)
                        {
                            Atualizar_Cronometro();
                            tempoAnterior = DateTime.Now;
                        }

                        // Se todas as letras foram encontradas, a variável 
                        // bProxima_Palavra será definida para true.
                        // Para irmos para a próxima palavra.
                        if (bProxima_Palavra)
                        {
                            bProxima_Palavra = false;
                            Exibir_Palavra(false);

                            // Vamos tentar recuperar a próxima palavra.
                            Proxima_Palavra();

                        }

                        // Na condição anterior, se bProxima_Palavra for true, ela executará a função
                        // Proxima_Palavra(), ela definirá a variável bTodas_Palavras_Lidas para true
                        // se não há mais palavras para ler.
                        if (bTodas_Palavras_Lidas)
                        {
                            Exibir_Tela_Pergunta();

                            if (bSair_do_Jogo)
                            {
                                return;
                            }
                                   
                        }                        

                        // Se o usuário pressionou alguma tecla verificar qual é.
                        if (Console.KeyAvailable)
                        {
                            Verificar_Teclas();
                        }

                        if (bReiniciar_Jogo)
                        {
                            Exibir_Tela_Pergunta();
                            bReiniciar_Jogo = false;
                            bAtualizar = true;
                            continue;
                        }

                        // Se o usuário perdeu o jogo
                        if (bPerdeu_Jogo)
                        {
                            Exibir_Tela_Pergunta();

                            if (bSair_do_Jogo)
                            {
                                return;
                            }
                        }

                        // Se o usuário pressionou esc, sair do jogo
                        if (bSair_do_Jogo)
                        {
                            Exibir_Tela_Pergunta();
                            bAtualizar = true;

                            if(bSair_do_Jogo)
                                return;
                        }
                    }

                }

            }
        }

        private ConsoleKey Aguardar_Resposta_Sim_Nao()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey cResposta = Console.ReadKey(true).Key;
                    if (cResposta == ConsoleKey.S || cResposta == ConsoleKey.N)
                    { 
                        return cResposta;
                    }
                }
            }
        }

        private void Exibir_Tela_Pergunta()
        {
            
            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DE_FUNDO;
            Console.Clear();

            int iTexto_Esquerda = 0;
            string strTexto = "";

            Console.SetCursorPosition(15, 15);
            if (bPerdeu_Jogo)
            {
                strTexto = "JOGAR NOVAMENTE? (S/N)";
                iTexto_Esquerda = (80 - strTexto.Length - 4) / 2;
                Desenhar_Borda_Dupla(" GAME OVER ", strTexto, iTexto_Esquerda, 15, 1);
                


            }
            else if(bTodas_Palavras_Lidas)
            {
                strTexto = "VOCÊ ACERTOU TODAS AS PALAVRAS!!! JOGAR NOVAMENTE? (S/N)";
                iTexto_Esquerda = (80 - strTexto.Length - 4) / 2;
                Desenhar_Borda_Dupla(" PARABÉNS ", strTexto, iTexto_Esquerda, 15, 1);
            }
            else if (bSair_do_Jogo)
            {
                strTexto = "TEM CERTEZA QUE DESEJA ABANDONAR O JOGO (S/N)?";
                iTexto_Esquerda = (80 - strTexto.Length - 4) / 2;
                Desenhar_Borda_Dupla("", strTexto, iTexto_Esquerda, 15, 1);
            }else if (bReiniciar_Jogo)
            {
                strTexto = "TEM CERTEZA QUE DESEJA ABANDONAR O JOGO E INICIAR UM NOVO (S/N)?";
                iTexto_Esquerda = (80 - strTexto.Length - 4) / 2;
                Desenhar_Borda_Dupla("", strTexto, iTexto_Esquerda, 15, 1);
            }

            if(bPerdeu_Jogo || bTodas_Palavras_Lidas)
            {
                if (Aguardar_Resposta_Sim_Nao() == ConsoleKey.S)
                {
                    bNovo_Jogo = true;
                    bSair_do_Jogo = false;
                    bPerdeu_Jogo = false;
                }
                else
                {
                    bNovo_Jogo = false;
                    bSair_do_Jogo = true;
                    bPerdeu_Jogo = false;
                }
            }
            else if(bSair_do_Jogo)
            {
                // A pergunta é: Tem certeza que deseja abandonar o jogo: (S/N)
                // Se sim, sair.
                if (Aguardar_Resposta_Sim_Nao() == ConsoleKey.S)
                {
                    bNovo_Jogo = false;
                    bSair_do_Jogo = true;

                    // Não iremos definir a variável bPerdeu_Jogo para falso pois
                    // Se o cronômetro do usuário expirar bPerdeu_Jogo será true.
                }
                else
                {
                    // Se não quer abandonar o jogo, quer continuar, então
                    // ambas variáveis para false.
                    bNovo_Jogo = false;
                    bSair_do_Jogo = false;
                }
            }else if (bReiniciar_Jogo)
            {
                // A pergunta é: Tem certeza que deseja abandonar o jogo e iniciar uma novo: (S/N)
                // Se sim, sair.
                if (Aguardar_Resposta_Sim_Nao() == ConsoleKey.S)
                {
                    bNovo_Jogo = true;
                    bSair_do_Jogo = false;
                    bPerdeu_Jogo = false;

                    // Não iremos definir a variável bPerdeu_Jogo para falso pois
                    // Se o cronômetro do usuário expirar bPerdeu_Jogo será true.
                }
                else
                {
                    // Se não quer abandonar o jogo, quer continuar, então
                    // ambas variáveis para false.
                    bNovo_Jogo = false;
                    bSair_do_Jogo = false;
                }
            }

            Console.Clear();
        }


		/// <summary>
		/// 	Este método verifica as teclas que o usuário deve utilizar no jogo.
		/// 	F1 - Reinicia o jogo;
		/// 	F2 - Ir para a próxima dica, se houver;
		/// 	Esc - Sai do jogo
		/// 	Verifica se a tecla é uma letra ou um número, se sim, o usuário chutou algum caractere.
		/// </summary>
		/// <returns>The teclas.</returns>
        private void Verificar_Teclas()
        {
            // Vamos capturar a tecla que foi pressionada.
            // Colocamos true, pois não queremos que a tecla apareça na tela.
            ConsoleKeyInfo tecla_pressionada = Console.ReadKey(true);

            switch (tecla_pressionada.Key)
            {
                case ConsoleKey.F1:
                    bReiniciar_Jogo = true;
                    break;

                // Se o usuário pressionou F2, ir para a próxima dica.
                case ConsoleKey.F2:
                    iErro++;
                    Exibir_Pontuacao();
                    iDica_Indice++;
                    Atualizar_Dica();
                    Atualizar_Boneco();
                    break;

                case ConsoleKey.Escape:
                    bSair_do_Jogo = true;
                    break;

                default:
					// Verifica se é uma tecla ou um número, se for, o usuário tentou adivinha alguma parte da palavra.
					if (Char.IsLetter(tecla_pressionada.KeyChar) || Char.IsNumber(tecla_pressionada.KeyChar))
                    {
                        char cCaractere = tecla_pressionada.KeyChar;
                        cCaractere = Char.ToLower(cCaractere);

                        string strPalavra = todas_palavras[iPalavra_Indice].strPalavra.ToLower();

                        // Se encontrarmos pelo menos 1 caractere em strPalavra, devemos definir como true.
                        bool bCaractere_Localizado = false;

                        // Vamos verificar se o caractere digitado pelo usuário existe em strPalavra.
						// Se o caractere digitado pelo usuário existe mais de uma vez na palavra, devemos 
						// marcá-lo como true.
                        for (int iA = 0; iA < strPalavra.Length; iA++)
                        {
                            if (strPalavra[iA] == cCaractere)
                            {
                                // Se a letra é igual, devemos verificar se esta posição já está selecionada.
                                // Se estiver, quer dizer que é um erro, devemos apagar uma parte do boneco.
                                if (bPalavra_Encontrada[iA] == false)
                                {
                                    bPalavra_Encontrada[iA] = true;
                                    iPonto += 10;
                                    bCaractere_Localizado = true;
                                }

                            }
                        }

                        // Quando saírmos do loop, devemos verificar, se algum caractere foi localizado.
						// Ou seja, só iremos chamar a função Exibir_Palavra, se algo foi encontrado.
                        if (bCaractere_Localizado)
                        {
                            Exibir_Palavra(true);
                        }
                        else
                        {  
							// O usuário não adivinhou a letra da palavra, então, devemos, atualizar o boneco
							// para indicar erro.
                            Atualizar_Boneco();
                            Desenhar_Boneco(false);
                            bAtualizar = true;
                        }

						// Devemos atualizar a pontuação.
                        Exibir_Pontuacao();


                        int quantidade_de_letras_encontradas = (from bPalavra in bPalavra_Encontrada
                                                 where bPalavra == true
                                                 select bPalavra).Count();
                        if (quantidade_de_letras_encontradas == bPalavra_Encontrada.Length)
                        {
                            bProxima_Palavra = true;
                        }

                    }
                    break;
            }
        }
       
		// As constantes abaixo são utilizadas para definir cada caractere
		// que será utilizado para desenhar a borda.
		private const char cBORDA_DUPLA_HORIZONTAL = (char)0x2550;
        private const char cBORDA_DUPLA_ESQUERDA_SUPERIOR = (char)0x2554;
        private const char cBORDA_DUPLA_DIREITA_SUPERIOR = (char)0x2557;
        private const char cBORDA_DUPLA_ESQUERDA_INFERIOR = (char)0x255a;
        private const char cBORDA_DUPLA_DIREITA_INFERIOR = (char)0x255d;

        private const char cBORDA_DUPLA_VERTICAL = (char)0x2551;
        private const char cBLOCO = (char)0x2588;
        private const char cBULLET = (char)0x2219;
        private const char cCIRCULO_BRANCO = (char)0x25cb;
        private const char cDUPLO_HORIZONTAL_VERTICAL = (char)0x256c;

        private const char cBORDA_SIMPLES_ESQUERDA_SUPERIOR = (char)0x250c;
        private const char cBORDA_SIMPLES_HORIZONTAL = (char)0x2500;
        private const char cBORDA_SIMPLES_DIREITA_SUPERIOR = (char)0x2510;
        private const char cBORDA_SIMPLES_VERTICAL = (char)0x2502;
        private const char cBORDA_SIMPLES_DIREITA_INFERIOR = (char)0x2518;
        private const char cBORDA_SIMPLES_ESQUERDA_INFERIOR = (char)0x2514;

        private const char cFACE_SORRINDO = (char)0x263b;

        // Quantos espaços antes ou após a palavra, se for zero, não haverá 
        // espaço.
        private const int iBORDA_ESPACO_ANTES_APOS_PALAVRA = 2;

        // Desenha uma borda ao redor do texto.
        // Tal borda terá um título.
        /// <summary>
        ///     Desenha uma borda ao redor do texto.
        /// </summary>
        /// <param name="strTitulo"></param>
        /// <param name="strTexto"></param>
        /// <param name="iEsquerda"></param>
        /// <param name="iTopo"></param>
        private void Desenhar_Borda_Dupla(string strTitulo, string strTexto, int iEsquerda, int iTopo, int iEspaco_Antes_Apos_Palavra)
        {
            int iCaractere_Texto_Quantidade = strTexto.Length + 2 + iEspaco_Antes_Apos_Palavra * 2;
            int iCaractere_Titulo_Quantidade = strTitulo.Length + 2 + iEspaco_Antes_Apos_Palavra * 2;

            // A borda sempre será desenhada, contornando sempre a maior quantidade de caracteres
            // Se strTitulo for maior irá considerá-lo, senão irá considerar o outro.
            if (iCaractere_Titulo_Quantidade > iCaractere_Texto_Quantidade)
                iCaractere_Texto_Quantidade = iCaractere_Titulo_Quantidade;
            

            int iCaracteres_Sem_Bordas = iCaractere_Texto_Quantidade - 2;


            // Borda superior
            Console.SetCursorPosition(iEsquerda, iTopo);
            Console.Write(cBORDA_DUPLA_ESQUERDA_SUPERIOR);
            Console.Write(new string(cBORDA_DUPLA_HORIZONTAL, iCaracteres_Sem_Bordas));

            //Console.Write("".PadLeft(iCaracteres_Sem_Bordas, cBORDA_DUPLA_HORIZONTAL));
            Console.Write(cBORDA_DUPLA_DIREITA_SUPERIOR);

            // Borda Inferior
            Console.SetCursorPosition(iEsquerda, iTopo + 2);
            Console.Write(cBORDA_DUPLA_ESQUERDA_INFERIOR);
            Console.Write(new string(cBORDA_DUPLA_HORIZONTAL, iCaracteres_Sem_Bordas));
            Console.Write(cBORDA_DUPLA_DIREITA_INFERIOR);

            // Borda lateral, antes da palavra e após a palavra, separada pela quantidade de caracteres informado
            // pelo usuário.
            Console.SetCursorPosition(iEsquerda, iTopo + 1);
            Console.Write(cBORDA_DUPLA_VERTICAL);

            // A posição a direita é:
            int iDireita = iEsquerda + iCaractere_Texto_Quantidade - 1;
            Console.SetCursorPosition(iDireita, iTopo + 1);
            Console.Write(cBORDA_DUPLA_VERTICAL);

            // Vamos inserir o título no centro horizontal da borda.
            int iTitulo_Posicao_Esquerda = (iCaractere_Texto_Quantidade - strTitulo.Length) / 2;
            Console.SetCursorPosition(iEsquerda + iTitulo_Posicao_Esquerda, iTopo);
            Console.Write(strTitulo);

            // Vamos inserir o texto no centro horizontal da borda.
            int iTexto_Posicao_Esquerda = (iCaractere_Texto_Quantidade - strTexto.Length) / 2;
            Console.SetCursorPosition(iEsquerda + iTexto_Posicao_Esquerda, iTopo + 1);
            Console.Write(strTexto);

        }

        // Desenha uma borda ao redor do texto.
        // Tal borda terá um título.
        /// <summary>
        ///     Desenha uma borda ao redor do texto.
        /// </summary>
        /// <param name="strTitulo"></param>
        /// <param name="strTexto"></param>
        /// <param name="iEsquerda"></param>
        /// <param name="iTopo"></param>
        private void Desenhar_Borda_Simples(string strTitulo, string strTexto, int iEsquerda, int iTopo, int iEspaco_Antes_Apos_Palavra)
        {
            int iCaractere_Texto_Quantidade = strTexto.Length + 2 + iEspaco_Antes_Apos_Palavra * 2;
            int iCaractere_Titulo_Quantidade = strTitulo.Length + 2 + iEspaco_Antes_Apos_Palavra * 2;

            // A borda sempre será desenhada, contornando sempre a maior quantidade de caracteres
            // Se strTitulo for maior irá considerá-lo, senão irá considerar o outro.
            if (iCaractere_Titulo_Quantidade > iCaractere_Texto_Quantidade)
                iCaractere_Texto_Quantidade = iCaractere_Titulo_Quantidade;


            int iCaracteres_Sem_Bordas = iCaractere_Texto_Quantidade - 2;


            // Borda superior
            Console.SetCursorPosition(iEsquerda, iTopo);
            Console.Write(cBORDA_SIMPLES_ESQUERDA_SUPERIOR);
            Console.Write(new string(cBORDA_SIMPLES_HORIZONTAL, iCaracteres_Sem_Bordas));

            //Console.Write("".PadLeft(iCaracteres_Sem_Bordas, cBORDA_SIMPLES_HORIZONTAL));
            Console.Write(cBORDA_SIMPLES_DIREITA_SUPERIOR);

            // Borda Inferior
            Console.SetCursorPosition(iEsquerda, iTopo + 2);
            Console.Write(cBORDA_SIMPLES_ESQUERDA_INFERIOR);
            Console.Write(new string(cBORDA_SIMPLES_HORIZONTAL, iCaracteres_Sem_Bordas));
            Console.Write(cBORDA_SIMPLES_DIREITA_INFERIOR);

            // Borda lateral, antes da palavra e após a palavra, separada pela quantidade de caracteres informado
            // pelo usuário.
            Console.SetCursorPosition(iEsquerda, iTopo + 1);
            Console.Write(cBORDA_SIMPLES_VERTICAL);

            // A posição a direita é:
            int iDireita = iEsquerda + iCaractere_Texto_Quantidade - 1;
            Console.SetCursorPosition(iDireita, iTopo + 1);
            Console.Write(cBORDA_SIMPLES_VERTICAL);

            // Vamos inserir o título no centro horizontal da borda.
            int iTitulo_Posicao_Esquerda = (iCaractere_Texto_Quantidade - strTitulo.Length) / 2;
            Console.SetCursorPosition(iEsquerda + iTitulo_Posicao_Esquerda, iTopo);
            Console.Write(strTitulo);

            // Vamos inserir o texto no centro horizontal da borda.
            int iTexto_Posicao_Esquerda = (iCaractere_Texto_Quantidade - strTexto.Length) / 2;
            Console.SetCursorPosition(iEsquerda + iTexto_Posicao_Esquerda, iTopo + 1);
            Console.Write(strTexto);
        }

		/// <summary>
		/// 	Método para desenhar a bola da tela.
		/// </summary>
		/// <returns></returns>
		/// <param name="strTitulo">O título a ser exibido na parte superior da janela</param>
        private void Desenhar_Borda_da_Janela(string strTitulo)
        {
            Console.SetCursorPosition(1, BORDA_JANELA_LINHA_SUPERIOR);
            Console.Write(new string(cBORDA_DUPLA_HORIZONTAL, 78));

            Console.SetCursorPosition(1, BORDA_JANELA_LINHA_INFERIOR);
            Console.Write(new string(cBORDA_DUPLA_HORIZONTAL, 78));

            for (int iA = 1; iA < BORDA_JANELA_LINHA_INFERIOR ; iA++)
            {
                Console.SetCursorPosition(0, iA);
                Console.Write(cBORDA_DUPLA_VERTICAL);
                Console.SetCursorPosition(79, iA);
                Console.Write(cBORDA_DUPLA_VERTICAL);
            }

            // Desenhar cantos.
            Console.SetCursorPosition(0, 0);
            Console.Write(cBORDA_DUPLA_ESQUERDA_SUPERIOR);
            Console.SetCursorPosition(79, 0);
            Console.Write(cBORDA_DUPLA_DIREITA_SUPERIOR);
            Console.SetCursorPosition(0, BORDA_JANELA_LINHA_INFERIOR);
            Console.Write(cBORDA_DUPLA_ESQUERDA_INFERIOR);
            Console.SetCursorPosition(79, BORDA_JANELA_LINHA_INFERIOR);
            Console.Write(cBORDA_DUPLA_DIREITA_INFERIOR);
        }


		/// <summary>
		/// 	Esta função atualiza a dica para que o usuário tente adivinha a palavra.
		/// 	Esta função é chamada toda vez que o usuário pressiona F2, se todas as dicas
		/// 	da palavra já foram exibidas, retorna para a primeira dica.
		/// 	Quando a palavra é descoberta, uma nova palavra é selecionada se houver e uma
		/// 	nova dica é exibida, também, se houver.
		/// </summary>
		/// <returns></returns>
        private void Atualizar_Dica()
        {
            // Vamos centralizar a palavra dica
            // A dica ficará abaixo da palavra.
            string strDica = "";

            if (iDica_Indice == todas_palavras[iPalavra_Indice].strDicas.Count)
            {
                iDica_Indice = 0;
            }

            if (todas_palavras[iPalavra_Indice].strDicas.Count != 0)
                strDica = todas_palavras[iPalavra_Indice].strDicas[iDica_Indice];
            else
                strDica = "<<NÃO HÁ DICA>>";

            // Apagar dica anterior, para não ficar lixo
            // Utilizar a cor da letra igual a cor de fundo
            Console.BackgroundColor = COR_DE_FUNDO;
            Console.ForegroundColor = COR_DE_FUNDO;

            int iDica_X;
            // Vamos centralizar a dica na tela abaixo da palavra
            if (strDica_Anterior != null)
            {
                // É 4, pois 1 para a borda esquerda, 1 para a borda direita
                // e 1 espaço antes e após a palavra. 
                iDica_X = (80 - strDica_Anterior.Length - 4) / 2;
                Desenhar_Borda_Dupla(" DICA ", strDica_Anterior, iDica_X, DICA_LINHA_SUPERIOR, 1);

            }

            // Imprimir dica atual na tela.
            Console.BackgroundColor = COR_DE_FUNDO;
            Console.ForegroundColor = COR_DA_LETRA;

            iDica_X = (80 - strDica.Length - 4) / 2;
            Desenhar_Borda_Dupla(" DICA ", strDica, iDica_X, DICA_LINHA_SUPERIOR, 1);

            // Apontar strDica para strDica_Anterior
            strDica_Anterior = strDica;

            // Sempre a cor do jogo é esta abaixo:
            Console.ForegroundColor = COR_DA_LETRA;
            Console.BackgroundColor = COR_DE_FUNDO;
        }

        
        private const int RODAPE_LINHA = BORDA_JANELA_LINHA_INFERIOR + 1;

		/// <summary>
		/// 	Exibe na parte inferior do console, as teclas que podem ser usadas para selecionar
		/// um novo jogo, exibir a próxima dica, ou se deseja sair do jogo.
		/// </summary>
		/// <returns>The rodape informativo.</returns>
        private void Exibir_Rodape_Informativo()
        {
            string[] strInformativo = new string[] { " F1 - NOVO JOGO ", " F2 - DICA ", "ESC - SAIR" };

            for(int iA = 0, iInformativo_Esquerda = 0; iA < strInformativo.Length; iA++)
            {
                if(iA == 0)
                {
                    iInformativo_Esquerda = 1;
                }
                else  if(iA == 1)
                {
                    iInformativo_Esquerda = (80 - 15) / 2;
                }
                else if(iA == 2)
                {
                    iInformativo_Esquerda = 79 - 15;
                }

                // Fundo branco, letras em azul.
                Console.ForegroundColor = COR_DE_FUNDO;
                Console.BackgroundColor = COR_DA_LETRA;

                Console.SetCursorPosition(iInformativo_Esquerda, RODAPE_LINHA);
                Console.Write(new string(' ', 15));

                Console.SetCursorPosition(iInformativo_Esquerda, RODAPE_LINHA);
                Console.Write(strInformativo[iA]);

            }            

        }

        // As letras das palavras são dispostas, conforme o valor da variável abaixo:
        private const int ESPACO_ENTRE_PALAVRAS = 3;

		/// <summary>
		/// 	Esta função é chamada toda vez que uma letra da palavra for adivinhada.
		/// Quando todas as letras forem adivinhadas, apaga todas as letras da tela para
		/// r para a próxima palavra, se houver.
		/// </summary>
		/// <returns>The palavra.</returns>
		/// <param name="bExibir_Palavra">Será true, se a palavra será exibir, entretanto, falso,
		/// se não deseja que seja exibida
		/// </param>
        private void Exibir_Palavra(bool bExibir_Palavra)
        {
            bAtualizar = !bAtualizar;



            int iPalavra_Comprimento = todas_palavras[iPalavra_Indice].strPalavra.Length;

            // Aqui, iremos centraliza o espaço na tela, para isso ocorrer
            // Devemos dividir o número de colunas na tela - menos a quantidade de espaço ocupado pelo
            // caractere e dividir esta diferença por 2.
            // Se o usuário colocar ESPACO_ENTRE_PALAVRAS igual a 1 por exemplo
            // Ao multiplicarmos teremos o mesmo valor, o que estaria errado, pois
            // se há um espaço entre as palavras então deveriamos multiplicar por 2, então neste
            // caso ESPACO_ENTRE_PALAVRAS é sempre adicionado 1.

            int iPosicao_X = (80 - (iPalavra_Comprimento * (ESPACO_ENTRE_PALAVRAS+1) - ESPACO_ENTRE_PALAVRAS + 1)) / 2;

            for (int iA = 0; iA < iPalavra_Comprimento; iA++)
            {

                // Se bExibir é igual a true, então queremos exibir a palavra na tela.
                if (bExibir_Palavra)
                {
                    Console.BackgroundColor = COR_DE_FUNDO;
                    Console.ForegroundColor = COR_DA_LETRA;
                }
                // Se bExibir é igual a falso, então não queremos exibir a palavra na tela,
                // Então, devemos imprimir o caractere na tela com a mesma cor de fundo.
                else
                {
                    Console.BackgroundColor = COR_DE_FUNDO;
                    Console.ForegroundColor = COR_DE_FUNDO;
                }



                Console.SetCursorPosition(iPosicao_X + iA, PALAVRA_LINHA_SUPERIOR);

                if (bPalavra_Encontrada[iA])
                {
                    // Fundo branco, letra azul.
                    if (bExibir_Palavra)
                    {
                        Console.BackgroundColor = COR_DA_LETRA;
                        Console.ForegroundColor = COR_DE_FUNDO;
                    }
                    Console.Write(todas_palavras[iPalavra_Indice].strPalavra[iA]);
                }
                else
                {
                    Console.Write("_");
                }

                iPosicao_X += ESPACO_ENTRE_PALAVRAS;
            }
        }

    	/// <summary>
    	/// 	O jogo deve ser executado no console, utilizando no mínimo uma largura de 80 colunas por 30 linhas.
    	/// </summary>
        void Ajustar_Console_Para_Jogo()
        {
            int Maximo_de_Colunas = Console.LargestWindowWidth;
            int Maximo_de_Linhas = Console.LargestWindowHeight;

            // Nosso jogo, será definido para 80 colunas por 35 linhas.
            //if(Maximo_de_Colunas < 80)
            //{
            //    throw new Exception("O jogo deve ter no mínimo 80 colunas.");
            //}

            Console.SetWindowPosition(0, 0);
            Console.WindowWidth = 80;
			Console.WindowHeight = 30;
        }


        static void Main(string[] args)
        {
            // Vamos pegar a configuração atual do console
            ConsoleColor cor_de_fundo = Console.BackgroundColor;
            ConsoleColor cor_da_letra = Console.ForegroundColor;

            Jogo_da_Forca forca = new Jogo_da_Forca();           
            forca.Jogar();

            Console.BackgroundColor = cor_de_fundo;
            Console.ForegroundColor = cor_da_letra;           
        }
    }
}
