# Iniciar com um clique

O pacote local **HORA-DA-FESTA-PRONTO** inclui o jogo Windows, o painel, o servidor e o Node portatil. Nao precisa abrir o Unity, instalar pacotes ou digitar comandos para jogar.

1. Na area de trabalho, abra **HORA DA FESTA - Iniciar Unity**.
2. O jogo e o painel abrem automaticamente. Use o painel para simular espectadores e presentes.
3. Para finalizar, abra **HORA DA FESTA - Encerrar Unity**.

Tambem e possivel usar INICIAR-HORA-DA-FESTA.cmd e ENCERRAR-HORA-DA-FESTA.cmd dentro do pacote. Mantenha a pasta inteira: o executavel precisa dos arquivos ao lado dele. Se mover a pasta, recrie os atalhos com scripts/Instalar-Atalhos.ps1.

O iniciador usa 8765 e reaproveita uma ponte compativel que ja esteja aberta. Se outra aplicacao ou uma ponte antiga ocupar essa porta, mostra uma mensagem e nao encerra o processo desconhecido. Cliques repetidos nao duplicam o jogo/servidor gerenciados.

Se o coletor existente em 8781 ja estiver conectado a uma LIVE, o iniciador detecta o usuario e ativa o encaminhamento automaticamente. Caso contrario, o simulador continua funcionando; conecte a LIVE no coletor existente quando desejar. O iniciador nao faz login no TikTok e nao encerra o coletor 8781. Mantenha uma aba do painel aberta durante a festa.

Os logs ficam em logs no pacote. O encerramento verifica o caminho do executavel e a data de inicio do processo para encerrar somente processos registrados pelo iniciador.

## Recriar o executavel

No Unity, use HORA DA FESTA > Criar jogo Windows. A saida padrao e Builds/Windows no projeto. Para um build automatizado, use scripts/Build-Windows.ps1, informando -UnityEditor, -ProjectPath e -Destination quando necessario. Use um caminho curto para o projeto, conforme UNITY-SINCRONIZACAO.md.

O codigo dos iniciadores fica no Git. Binarios Windows e Node sao entregues no pacote local e nao sao adicionados ao repositorio. Para validar o pacote, execute scripts/Test-Iniciador.ps1 -PackagePath com a pasta PRONTO.
