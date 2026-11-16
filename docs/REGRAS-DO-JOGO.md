# HORA DA FESTA — Regras do Arraiá dos Insetos

## Modos
- Modo 1: `!entrar` gratuito, Rosa e GG também adicionam o espectador.
- Modo 2: entrada por presentes; `!entrar` desativado.
- Participantes são únicos por identificador do espectador.

## Presentes
- Rosa: adiciona à festa; 15 Rosas acumuladas pelo mesmo espectador promovem a noivo/noiva e reiniciam o contador de Rosas.
- GG: adiciona à festa.
- Coração com dedos: promove para a banda.
- Hora do Café (15 moedas, conforme catálogo consultado pelo criador): promove a noivo/noiva.
- Presentes acima de 15 moedas: promovem a noivo/noiva, exceto presentes com efeitos específicos.
- Perfume: fogos de artifício por 10 segundos.
- Rosquinha: Quadrilha Maluca por 30 segundos.
- Efeitos específicos prevalecem sobre regras genéricas por valor.

## Banda
Seis vagas: sanfona, zabumba, triângulo, saxofone, teclado e violão. Ao entrar o sétimo músico, sai o mais antigo (FIFO), que retorna à quadrilha. Não duplicar participantes nem instrumentos.

## Noivos e câmera
Noivo e noiva são papéis atribuídos aleatoriamente, sem inferir gênero. Quando substituídos, os anteriores voltam à quadrilha. Câmera automática com prioridade de eventos, foco em presentes e enquadramento amplo durante fogos. Durações configuráveis no painel.

## Estado atual
Ponte local implementada em 8765, reaproveitando o coletor LIVE existente em 8781. O painel aplica estas regras e publica o estado ao Unity. A validação com uma LIVE real permanece pendente; o simulador e os testes locais não precisam conectar ao TikTok.
