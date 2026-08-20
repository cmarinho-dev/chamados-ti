# Chamados TI

Este sistema concentra informações de diversos dispositivos do imap, tais como computadores, notebooks e impressoras;
Além da gerência destes dispositivos há o recurso de chamados, onde quaisquer problema de TI pode chamar a nossa equipe do NIT através deste sistema;

## Localizao do Sistema no Filezilla

Ele se encontra no servidor FTP de nossas aplicações no endereço 
`/sistemas/chamadosti`

## Como rodar
Observar perfil do app em `/Properties/launchSettings.json`

Parar rodar localmente você deve alternar para perfil dev:

- Alternar http e https para perfil de desenvolvimento:
- > "ASPNETCORE_ENVIRONMENT": "Development"

Para produção (quando for fazer deploy no servidor ftp):

- Alternar http e https para perfil de produção:
- > "ASPNETCORE_ENVIRONMENT": "Production"

Você deve também verificar a conexão com banco e login e senha de acesso do app em
`appsettings.Development.json` ou `appsettings.Production.json` conforme o perfil de ambiente.


### Comandos
Rodar localmente **(deve ter .NET SDK 9 instalado)**:
> dotnet run

### Atualizar aplicação de produção

Antes de publicar, confirme se o arquivo `appsettings.Production.json` está com a conexão do banco e as credenciais corretas para produção. Não altere `appsettings.Development.json` para fazer deploy.

Gerar os arquivos de publicação **(deve ter .NET SDK 9 instalado)**:

> dotnet publish -c Release -p:PublishProfile=FolderProfile -o "./_topublish"

O comando acima publica a aplicação na pasta local:

> `_topublish`

No FileZilla, acessar o diretório de produção:

> `/sistemas/chamadosti`

Enviar para o servidor **somente o conteúdo da pasta `_topublish`**, sobrescrevendo os arquivos existentes em `/sistemas/chamadosti`.

Checklist rápido após o deploy:

- Confirmar que o ambiente do servidor está como `Production`.
- Confirmar que `appsettings.Production.json` foi publicado com os dados corretos.
- Confirmar que a aplicação abriu sem erro.
- Fazer login administrativo e testar uma consulta simples.
