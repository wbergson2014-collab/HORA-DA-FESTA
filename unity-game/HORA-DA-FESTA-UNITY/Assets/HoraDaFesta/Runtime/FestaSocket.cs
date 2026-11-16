using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace HoraDaFesta {
public class FestaSocket : MonoBehaviour {
 public string url="ws://127.0.0.1:8765";
 public readonly ConcurrentQueue<string> Messages=new ConcurrentQueue<string>();
 public string Status {get; private set;}="Desconectado";
 CancellationTokenSource stop; ClientWebSocket socket;
 void OnEnable(){stop=new CancellationTokenSource();_ = Run(stop.Token);}
 async Task Run(CancellationToken token){
 while(!token.IsCancellationRequested){
 try { using(var client=new ClientWebSocket()) { socket=client;Status="Conectando";await client.ConnectAsync(new Uri(url),token);
 var hello=Encoding.UTF8.GetBytes("{\"action\":\"unity-register\"}");await client.SendAsync(new ArraySegment<byte>(hello),WebSocketMessageType.Text,true,token);Status="Conectado";
 byte[] buffer=new byte[8192];while(client.State==WebSocketState.Open&&!token.IsCancellationRequested){using(var data=new MemoryStream()){WebSocketReceiveResult result;do{result=await client.ReceiveAsync(new ArraySegment<byte>(buffer),token);if(result.MessageType==WebSocketMessageType.Close)break;data.Write(buffer,0,result.Count);if(data.Length>1024*1024)throw new IOException("Mensagem grande demais");}while(!result.EndOfMessage);if(result.MessageType==WebSocketMessageType.Close)break;if(result.MessageType==WebSocketMessageType.Text)Messages.Enqueue(Encoding.UTF8.GetString(data.ToArray()));}}
 }}catch(OperationCanceledException){break;}catch(Exception e){Status="Reconectando: "+e.Message;}
 try{await Task.Delay(2000,token);}catch(OperationCanceledException){break;}
 }Status="Desconectado";
 }
 void OnDisable(){stop?.Cancel();socket?.Abort();stop?.Dispose();}
}
}
