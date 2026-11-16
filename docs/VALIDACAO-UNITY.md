# Validação desta implementação

Ambiente: Unity 6000.6.9f1, URP 17.6.0, Windows 11, Node.js 24.20.0.

- npm test: 2 testes aprovados. Painel em DOM controlado e ponte com WebSockets reais e coletor HTTP isolado; entrada, modo 2, 15 rosas, FIFO, transição de papéis, efeitos específicos, replay, comandos e eventos LIVE simulados.
- Unity EditMode: 2/2 aprovados no Test Runner. Desserialização do contrato e geração da cena com URP e configuração de build.
- Unity PlayMode: 1/1 aprovado no Test Runner. Criação do cenário, identidade/reutilização dos atores, manutenção das vagas por instrumento, promoção a noiva e reset.
- Runtime, Editor e testes C# também compilados separadamente com Roslyn e bibliotecas locais do Unity instalado.
- Sync-Unity.ps1 executado somente em prévia. Nenhum arquivo do projeto Unity pessoal em D: foi copiado, alterado ou sobrescrito.

## Problemas resolvidos no ambiente de teste
As primeiras execuções não encontravam as variáveis padrão do Windows e o Package Manager não iniciava. Ao fornecê-las ao processo isolado, a importação avançou. Em seguida, caminhos de pacotes com 279 caracteres causavam erros Host type is not matching any asset type em UXML e shaders URP. A configuração final usa os módulos padrão do Unity 6 e uGUI; a importação nova por um alias curto do mesmo checkout aprovou EditMode e PlayMode. Não foram suprimidos erros nos testes.

Use um checkout curto no Windows (por exemplo C:\HDF), não reutilize Library importada em um caminho longo e siga UNITY-SINCRONIZACAO.md. A cena e os assets URP gerados estão incluídos; Library e caches não estão no Git.

## Limites da validação
O executável Windows foi gerado e abriu pelo iniciador. Uma revisão visual básica conferiu bandeirinhas coloridas, banda, noivos e 26 participantes de teste recebidos por WebSocket. O iniciador passou em verificação de arquivos, início, segunda abertura sem duplicação e encerramento; o coletor 8781 permaneceu em execução. A LIVE real não foi testada. O protótipo usa geometria/animação procedural e não inclui áudio nem modelos artísticos finais. Mantenha uma aba do painel aberta para aplicar as regras e sortear papéis uma única vez.
