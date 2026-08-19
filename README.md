# stock-quote-alert

Aplicação de console em C# que monitora a cotação de um ativo da B3 e envia um alerta por e-mail quando o preço ultrapassa o limite de venda ou fica abaixo do limite de compra.

A aplicação consulta a API de cotação [brapi](https://brapi.dev) periodicamente e utiliza SMTP para envio dos alertas.

## Como executar

### Pré-requisito

* [.NET 10](https://dotnet.microsoft.com/download)

Clone o projeto:

```bash
git clone git@github.com:danielbbr/stock-quote-alert.git
cd stock-quote-alert
```

Configure o token da brapi (gere um token gratuito em https://brapi.dev):

```bash
dotnet user-secrets set "Brapi:Token" "SEU_TOKEN" --project StockQuoteAlert
```

Configure o servidor SMTP que vai receber os alertas em `appsettings.json`:

```json
"Smtp": {
  "To": "client@client.com",
  "Host": "localhost",
  "Port": 1025,
  "EnableSsl": false,
  "From": "server@stockquotealert.com",
  "User": "",
  "Password": ""
}
```

A configuração padrão aponta para o [Mailpit](https://mailpit.axllent.org), que permite testar sem um servidor real. Com o Docker instalado:

```bash
docker run -d -p 1025:1025 -p 8025:8025 axllent/mailpit
```

Os e-mails enviados ficam visíveis em http://localhost:8025.

Execute:

```bash
dotnet run --project StockQuoteAlert -- PETR4 22.67 22.59
```

O formato dos argumentos é:

```text
<ativo> <precoVenda> <precoCompra>
```

## Funcionamento

A cotação é consultada a cada 60 segundos por padrão. O intervalo é configurável em `appsettings.json`:

```json
"Monitoring": {
  "IntervalSeconds": 60
}
```

A cada ciclo, a cotação é classificada como:

* **Venda:** acima do limite de venda.
* **Compra:** abaixo do limite de compra.
* **Neutra:** entre os dois limites.

O alerta é enviado apenas quando o preço **entra** em uma zona de compra ou venda, evitando o envio repetido enquanto o preço permanece além do limite.

## Testes

```bash
dotnet test
```

Incluí uma suíte básica de testes em xUnit, cobrindo os principais casos de cada componente: validação de argumentos, classificação de preços, ciclo de monitoramento e integração com a brapi.

## Uso de IA

Foi utilizado **Claude** para consulta de exemplos de código, documentação e boilerplate.

A implementação, lógica, decisões técnicas, arquitetura, e escopo são de minha autoria.
