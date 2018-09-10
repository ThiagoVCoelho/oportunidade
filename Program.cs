using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel;

 namespace ProjetoMinutoSeguros
{
    //A listagem de preposições e artigos foi seguido alguns sites dos quais falavam do assunto
    // https://www.soportugues.com.br/secoes/morf/morf80.php
    // https://gramaticaportuguesa.blogs.sapo.pt/5865.html

    class PreposicoesEArtigos 
     {
         public string[] preposicoesEartigos = {"o",	
                                                "a",
                                                "os",	
                                                "as",
                                                "um",	
                                                "uma",	
                                                "uns",	
                                                "umas",
                                                "a",
                                                "ao",	
                                                "à",
                                                "aos",	
                                                "às",
                                                "de",
                                                "do",
                                                "da",
                                                "dos",	
                                                "das",	
                                                "dum",	
                                                "duma",	
                                                "duns",
                                                "dumas",
                                                "em",
                                                "no",	
                                                "na",	
                                                "nos",	
                                                "nas",	
                                                "num",	
                                                "numa",	
                                                "nuns",
                                                "numas",
                                                "por",
                                                "pelo",	
                                                "pela",
                                                "pelos",
                                                "pelas",
                                                "é",
                                                "mais",
                                                "e",
                                                "com",
                                                "para",
                                                "que",
                                                "como",
                                                "não",
                                                "quais",
                                                "ante",
                                                "após",
                                                "até",
                                                "com",
                                                "conforme",
                                                "contra",
                                                "consoante",
                                                "de",
                                                "desde",
                                                "durante",	
                                                "em",
                                                "excepto",
                                                "entre",
                                                "mediante",
                                                "para",
                                                "perante",
                                                "por",
                                                "salvo",
                                                "sem",
                                                "segundo",
                                                "sob",
                                                "sobre",
                                                "trás",
                                                ""};
     }
     
    class Topicos
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Conteudo { get; set; }
    }
    
    class Nomalizado
    {
        public string TextoLimpo{ get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Método criado para sanar erro "A conexão subjacente estava fechada: Erro inesperado em um envio."
                //no qual busca os protocolos do wndows padrão para efetuar o post
                //1 - Remove os protocolos do windows 
                desvincula_Protocolos();

                //URL to get Feed
                string urlFeed = "https://www.minutoseguros.com.br/blog/feed/";

                //Lista os tópicos
                List<Topicos> topicos = LerFeedXML(urlFeed);

                //Remove pontuações, preposições e quebra de linhas do html
                List<Nomalizado> normalizado = RemoveSujeiras(topicos);
            
                //Saída
                //Imprime a primeira parte do desafio
                Console.WriteLine("Primeira parte do desafio - Obter automaticamente o conteúdo dos dez últimos tópicos publicados no blog da Minuto Seguros.");
                Console.WriteLine(LerTitulos(topicos));
                Console.WriteLine("####################################");
                
                //Imprime a segunda parte do desafio
                Console.WriteLine("Segunda parte do desafio - Quais as dez principais palavras abordadas por tópico nesses tópicos e qual o número de vezes que elas aparecem.");
                Console.WriteLine(PalavrasTop10(normalizado));
                Console.WriteLine("####################################");

                //Imprimie a terceira parte do desafio
                Console.WriteLine("Terceira parte do desafio - Exibir a quantidade de palavras por tópico");
                Console.WriteLine(Qtde_Palavras(normalizado));
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao executar o Desafio: " + ex.Message);
            }
        }

        private static string LerTitulos(List<Topicos> topicos)
        {
            try
            {
                int i = 1;

                StringBuilder sb = new StringBuilder();
                foreach (Topicos topico in topicos)
                {
                    sb.Append(string.Format("- Tópico {0} - {1} palavras distintas.\n", i, topico.Titulo));
                    i++;
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler os títulos (primeira parte do desafio): " + ex.Message);
            }

        }

        private static string PalavrasTop10(List<Nomalizado> normalizado)
        {
            try
            {
                string text = "";

                foreach (Nomalizado norm in normalizado)
                    text = text + norm.TextoLimpo;

                var palavras = text.Trim().Split(' ').Select(p => p).ToList();

                var contador = text.Trim().Split(' ').Select(p => new KeyValuePair<string, int>(p, palavras.Count(a => a.Equals(p)))).Distinct().ToList();

                var top10 = contador.OrderByDescending(p => p.Value).Take(10).ToList();
                
                StringBuilder sb = new StringBuilder();

                int i = 1;
                foreach (KeyValuePair<string, int> palavra in top10)
                {
                    sb.Append(string.Format("{0} - A Palavra '{1}' - repete: {2} vezes.\n", i, palavra.Key, palavra.Value));
                    i++;
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar Top 10 (Segunda parte do desafio): " + ex.Message);
            }
        }

        private static string Qtde_Palavras(List<Nomalizado> normalizado)
        {
            try
            {
                int i = 1;

                StringBuilder sb = new StringBuilder();

                foreach (Nomalizado norm in normalizado)
                {
                    var Palavras_distintas = norm.TextoLimpo.Trim().Split(' ').Select(p => p).Distinct().ToList();
                    sb.Append(string.Format("O Tópico '{0}' possui {1} palavras.\n", i, Palavras_distintas.Count()));

                    i++;
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar as quantidade de palavras (Terceira parte do Desafio): " + ex.Message);
            }

        }
        
        static void desvincula_Protocolos()
        {
            // Remover protocolos padrão da máquina
            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Ssl3))
            {
                ServicePointManager.SecurityProtocol &= ~(SecurityProtocolType.Ssl3);
            }
            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls))
            {
                ServicePointManager.SecurityProtocol &= ~(SecurityProtocolType.Tls);
            }
            if (ServicePointManager.SecurityProtocol.HasFlag((SecurityProtocolType)768)) // TLS 1.1
            {
                ServicePointManager.SecurityProtocol &= ~((SecurityProtocolType)768);
            }
            if (ServicePointManager.SecurityProtocol.HasFlag((SecurityProtocolType)3072)) // TLS 1.2
            {
                ServicePointManager.SecurityProtocol &= ~((SecurityProtocolType)3072);
            }

            // Pilha com protocolos que vou utilizar
            Stack<SecurityProtocolType> protocolosDisponiveis = new Stack<SecurityProtocolType>();
            protocolosDisponiveis.Push(SecurityProtocolType.Tls);
            protocolosDisponiveis.Push((SecurityProtocolType)768); // TLS 1.1
            protocolosDisponiveis.Push((SecurityProtocolType)3072); // TLS 1.2.
            var TentarNovamente = false;

            do
            {
                var protocolType = protocolosDisponiveis.Pop();
                ServicePointManager.SecurityProtocol = protocolType;

                try
                {
                    // Tenta chamar o serviço se der certo ok eu saio fora
                    TentarNovamente = false;
                }
                catch (CommunicationException ex)
                {
                    TentarNovamente = true;
                    // Remover o protocolo utilizado para o Framework não tentar utilizar 
                    if (ServicePointManager.SecurityProtocol.HasFlag(protocolType))
                    {
                        ServicePointManager.SecurityProtocol &= ~(protocolType);
                    }
                }
            } while (TentarNovamente && protocolosDisponiveis.Count() > 0);
        }

        private static List<Topicos> LerFeedXML(string url)
        {
            try
            {
                XDocument xml = XDocument.Load(url);
                return xml.Root
                   .Elements("channel")
                   .Elements("item")
                   .Select(p => new Topicos
                   {
                       Titulo = p.Element("title").Value,
                       Descricao = p.Element("description").Value,
                       Conteudo = p.Element(XName.Get("encoded", "http://purl.org/rss/1.0/modules/content/")).Value
                   })
                   .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar o Feed: " + ex.Message);
            }
        }
        
        private static List<Nomalizado> RemoveSujeiras(List<Topicos> topicos)
        {
            try
            {
                return topicos.Select(
                    p => new Nomalizado
                    {
                        TextoLimpo = string.Format("{0} {1} {2}",
                        RemoverPreposicoesArtigos(RemoverDiversos(p.Titulo.ToLower().Trim())),
                        RemoverPreposicoesArtigos(RemoverDiversos(p.Descricao.ToLower().Trim())),
                        RemoverPreposicoesArtigos(RemoverDiversos(p.Conteudo.ToLower().Trim()))
                        ).Trim()
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover sujeras (normalizar os textos): " + ex.Message);
            }
        }
               
        private static string RemoverPreposicoesArtigos(string text)
        {
            try
            {
                PreposicoesEArtigos prepo = new PreposicoesEArtigos();

                return string.Join(" ", text.Split(' ').Except(prepo.preposicoesEartigos));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover preposições e artigos: " + ex.Message);
            }
        }

        private static string RemoverDiversos(string text)
        {
            try
            {
                string texto;

                //remove as pontuações
                texto = new string(text.Where(p => !char.IsPunctuation(p)).ToArray());

                //remove quebras de linha
                string SeparadorLinha = ((char)0x2028).ToString();
                string SeparadorParagrafo = ((char)0x2029).ToString();
                texto.Replace("\r\n", " ")
                     .Replace("\n", " ")
                     .Replace("\r", " ")
                     .Replace(System.Environment.NewLine, " ")
                     .Replace(SeparadorLinha, string.Empty)
                     .Replace(SeparadorParagrafo, string.Empty);

                //removetags
                texto = Regex.Replace(text, "<.*?>", string.Empty);

                return texto;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover diversos (pontuações, quebras de linha e tags do HTML): " + ex.Message);
            }
        }
        
    }
}
