# Unity 6 — instalar e testar

## Estado e limites
Implementação procedural com primitivas 3D, seis instrumentos visuais, dança por movimentos de juntas, quadrilha em quatro fases de oito segundos (filas, aproximação, roda, troca de lados), noivos e nomes. Não contém modelos artísticos importados, áudio de forró ou captura de movimento. Os seis músicos iniciais são figurantes; cada vaga ocupada substitui seu figurante. Os movimentos usam um relógio comum; não dependem de um Animator ou de clipes externos.

O projeto local foi apenas inspecionado nesta sessão: Unity 6000.6.9f1, URP 17.6.0. Não foi sincronizado nem sobrescrito. O trabalho e os testes usam a cópia do repositório.

## Testar a cópia independente (recomendado)
1. Faça checkout da branch com estas alterações em uma pasta separada. Instale Node.js 24 LTS e Unity 6000.6.9f1 com suporte Windows.
2. Em PowerShell, na raiz do repositório:

```powershell
cd tiktok-integration
npm ci
npm test
npm start
```

3. Abra painel-admin/index.html no navegador. Mantenha somente uma aba de painel aberta: ela é a autoridade das regras e conserva seu estado no localStorage. Não é necessário conectar uma LIVE para simular.
4. No Unity Hub, Add > Add project from disk, selecione unity-game/HORA-DA-FESTA-UNITY. Aguarde importação/compilação.
5. Execute o menu **HORA DA FESTA > Gerar cena Arraial**. O menu pede que salve qualquer cena aberta, cria uma cena separada em Assets/HoraDaFesta/Generated/Arraial.unity, configura URP e define essa cena para build. Na cópia independente isso é esperado; em um projeto pessoal use backup antes de executar o menu.
6. Pressione Play. O cenário é montado ao iniciar; o indicador Unity do painel deve ficar conectado. O cliente registra unity-register e reconecta a cada dois segundos.
7. Simule !entrar, 15 rosas, sete corações com dedos, café, perfume e rosquinha. Confira nomes, substituição FIFO da banda, noivos sem duplicação, fogos e dança acelerada. Perfume deve gerar fogos mesmo com valor alto, sem promover a noivo. Modo 2 deve bloquear !entrar. Durações de efeitos vêm dos campos do painel.
8. Teste câmeras Noivos, Banda, Visão geral e Automática. Fogos têm prioridade de visão ampla; presentes reais focam o espectador por quatro segundos em modo automático. Após o prazo, a câmera retorna ao percurso automático.
9. Pare e reinicie Play mantendo a ponte e o painel. O último estado deve reaparecer. Se reiniciar a ponte, o painel reconecta em até quatro segundos e publica novamente. Os efeitos usam prazo absoluto e não reiniciam sua duração.
10. Para LIVE, mantenha seu conector existente em 8781; no painel informe o mesmo usuário e clique Conectar TikTok. A ponte consulta /roblox/events com session/after; ao mudar a sessão, posiciona o cursor no fim para evitar repetir presentes antigos. Não cria uma segunda conexão LIVE. Validar presentes reais requer uma LIVE disponível; os testes usam um coletor falso isolado.

## Sincronização opcional com o projeto pessoal
Faça backup e feche o Unity local. Na raiz do repositório, veja a prévia:

```powershell
powershell -NoProfile -File .\scripts\Sync-Unity.ps1
```

Se a origem e o destino estiverem corretos, execute:

```powershell
powershell -NoProfile -File .\scripts\Sync-Unity.ps1 -Apply
```

Destino padrão: D:\PROJETOS GPT\HORA DA FESTA\HORA-DA-FESTA-UNITY. O script copia somente Runtime e Editor para uma pasta Assets/HoraDaFesta nova; recusa sobrescrever uma pasta existente. Não copia cenas pessoais, Library, Packages ou ProjectSettings. Para outra pasta, use -Destination 'caminho'. Depois abra Unity e gere a cena pelo menu. O menu altera a configuração de render e a lista de cenas de build; faça isso em uma cópia se precisar manter essas configurações pessoais.

Para atualizar uma instalação já existente, compare e mescle Assets/HoraDaFesta em uma cópia de segurança; o script deliberadamente não substitui versões anteriores. Nunca copie o projeto inteiro por cima do projeto pessoal.

## Testes Unity
Na cópia independente, Window > General > Test Runner > EditMode > Run All e depois PlayMode > Run All. Os testes verificam desserialização, geração da cena com URP, reutilização de atores, vagas de instrumentos e reset. Para executar por linha de comando:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.6.9f1\Editor\Unity.exe' -batchmode -nographics -projectPath "$PWD\unity-game\HORA-DA-FESTA-UNITY" -runTests -testPlatform EditMode -testResults "$PWD\unity-results.xml" -logFile "$PWD\unity-tests.log"
```

Não acrescente -quit a -runTests. Esses testes não substituem conferir a apresentação em Play e testar uma LIVE real. O alvo é Windows Editor/Standalone; ClientWebSocket não atende WebGL.
