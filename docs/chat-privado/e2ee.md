---
layout: default
title: E2EE
parent: Chat privado / E2E
nav_order: 3
---

# Cifrado de extremo a extremo (E2EE)

El E2EE garantiza que el contenido sensible de una conversación no se almacene en texto plano en el servidor. En este proyecto el cifrado en reposo usa **AES** con clave de 256 bits por conversación.

## Algoritmo y almacenamiento de claves

- Implementación: clase estática [`E2EEncryptionService`](https://github.com/DaR3kDev/uni-chat-backend/blob/main/uni-chat-backend/Infrastructure/Security/E2EEncryptionService.cs) en `Infrastructure/Security/`.
- Al crear una conversación directa, `GetOrCreateConversationHandler` genera una clave con `E2EEncryptionService.GenerateKey()` y la guarda en `Conversation.EncryptionKey` (Base64 en MongoDB).
- Solo los mensajes de tipo **TEXT** con contenido no vacío se cifran antes de persistir.

## Flujo de cifrado

```
  Cliente (emisor)              Backend                         Cliente (receptor)
        │                          │                                    │
        │  texto plano (HTTPS)     │                                    │
        │─────────────────────────►│                                    │
        │                   GetEncryptionKeyAsync                      │
        │                   E2EEncryptionService.Encrypt               │
        │                   guardar content cifrado en MongoDB         │
        │                   SignalR ReceiveMessage (payload según API) │
        │                          │──────────────────────────────────►│
        │                          │                          descifrado
        │                          │                          en cliente*
```

\* La API puede devolver contenido descifrado en lecturas HTTP (`GetMessagesHandler` usa `Decrypt` al armar la respuesta). Ajusta el contrato del cliente según tu modelo de confianza.

### Etapas

1. El usuario envía texto por HTTP o SignalR (`SendMessage`).
2. `SendMessageHandler` obtiene la clave de la conversación y llama a `Encrypt(plainText, keyBytes)`.
3. MongoDB persiste `Message.Content` cifrado (Base64: IV + ciphertext).
4. Eventos en tiempo real y lecturas posteriores usan la misma clave de conversación.

## Implementación real (resumen)

```csharp
public static class E2EEncryptionService
{
    public static byte[] GenerateKey() { /* Aes.Create().GenerateKey() */ }

    public static string Encrypt(string plainText, byte[] key)
    {
        // IV de 16 bytes prefijado al ciphertext, todo en Base64
    }

    public static string Decrypt(string cipherText, byte[] key)
    {
        // Extrae IV, descifra con AES-CBC vía CryptoStream
    }
}
```

Archivo completo en el repositorio: `uni-chat-backend/Infrastructure/Security/E2EEncryptionService.cs`.

## Garantías y límites

| Escenario | Comportamiento |
|-----------|----------------|
| Tránsito | HTTPS + JWT en API y SignalR |
| Repositorio | `content` cifrado para texto |
| Por conversación | Clave distinta (`EncryptionKey` en `Conversation`) |
| Archivos multimedia | Se almacenan URL en Cloudinary; no pasan por el mismo cifrado de texto |

{: .important }
> **Importante:** No persistas ni registres en logs el texto plano de mensajes. El pre-commit del repo ayuda a evitar commitear `.env` y secretos.

## Otras medidas de seguridad

| Mecanismo | Función |
|-----------|---------|
| JWT + Refresh Token | Autenticación y renovación de sesión |
| `[Authorize]` en hub y endpoints | Acceso solo con token válido |
| Rate limiting | Mitigación de abuso en la API |
| Secretos fuera del repo | GitHub Secrets + `render-env.sh` — ver [CI/CD y secrets]({{ site.baseurl }}/github-secrets.html) |
