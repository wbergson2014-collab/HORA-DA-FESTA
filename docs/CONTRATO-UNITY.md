# Contrato painel / ponte / Unity (v1)

Endereço padrão: ws://127.0.0.1:8765. Somente loopback. Mensagens JSON UTF-8, limite de 1 MiB. Windows Editor/Standalone; não WebGL.

## Mensagens preservadas
- Unity envia {"action":"unity-register"} a cada conexão. O servidor atualiza status para os painéis.
- Painel envia status, tiktok-connect (username), tiktok-disconnect conforme a versão existente.
- game-command continua encaminhando command/payload ao Unity. Comandos implementados: camera com payload.mode (automatic, wide, couple, band); fireworks/dance com payload.seconds (1–120). O próximo estado do painel prevalece sobre comandos transitórios.
- tiktok-event mantém kind, user, gift, count, coins e name; agora é entregue tanto a painéis quanto a Unity. Unity usa esse evento para foco; as regras são aplicadas uma única vez pelo painel e chegam no estado seguinte.

## Estado completo em vez de reaplicar regras no Unity
Após toda renderização que muda o estado e após reconectar, o painel publica:

```json
{"action":"panel-state","state":{"mode":1,"people":[{"name":"alice","roses":0,"role":"banda"}],"band":[{"id":"alice","instrument":"Sanfona"}],"couple":{"noivo":null,"noiva":null},"camera":"automatic","effect":{"kind":"fogos","until":1893456000000},"focus":{"user":"alice","until":1893455994000}}}
```

people é um ARRAY no transporte (o painel conserva internamente um objeto por id). band é ordenada FIFO; cada campo instrument determina a vaga visual mesmo se alguém deixar a banda para virar noivo. Efeitos: fogos ou quadrilha; effect e focus podem ser null. until é Unix UTC em milissegundos; Unity verifica prazo, evitando reiniciar efeitos na reconexão. Papéis: quadrilha, banda, noivo, noiva. Identificador é o handle normalizado já usado pelo painel. A fonte 8781 preserva callerHandle/uniqueId/userId/id e o cursor original.

A ponte responde aos Unity registrados com {type:"game-state",revision:N,state:...}. Mantém o último estado em memória e o reenvia imediatamente ao registrar Unity. revision é informativa, reinicia com a ponte; Unity aceita o estado de cada nova conexão. Não se usa a revisão para descartar mensagens de uma ponte reiniciada.

Somente sockets com papel panel podem publicar estado ou comandos. Validação limita modo, campos principais, número de pessoas (2000) e banda (6). Não há autenticação; mantenha a escuta local. Use uma aba de painel por festa: múltiplos painéis têm estados locais independentes e a última publicação prevalece. Fechar todos os painéis impede aplicar novas regras LIVE; o servidor apenas transporta eventos. Isso preserva a lógica existente e evita sorteios de noivos divergentes entre o painel e o jogo.

## Coletor 8781 preservado
HTTP /roblox/events?session=...&after=... com live, username, session, cursor, events. Primeira consulta/mudança de sessão alinha o cursor ao fim; novas consultas encaminham os deltas. Não abre conexão TikTok adicional, não altera o coletor nem instala credenciais. SOURCE_URL e PORT permitem isolar os testes; os padrões continuam 8781 e 8765.

## Recuperação
- Unity reinicia: registra novamente e recebe o último snapshot.
- Ponte reinicia: painel reconecta em 4 s e republica; Unity em 2 s.
- Painel recarrega: restaura localStorage e republica.
- Reset no painel: envia people/band vazios, remove os atores reais e restaura figurantes.
- Falta do coletor: status error/notice; a simulação local continua disponível.

## Identificacao para o iniciador
As mensagens status incluem bridge: "hora-da-festa" e protocolVersion: 1. O iniciador valida esses campos antes de reutilizar uma ponte na porta 8765 e nao encerra processos desconhecidos.
