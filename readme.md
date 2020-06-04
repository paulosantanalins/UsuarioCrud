# USUARIO CRUD

CRUD Simples implementado com .NET Core e Angular 7.

## Instalação Angular

Dirija-se ao diretório Angular e use o seguinte comando

```node
npm install 
npm start
```

## Uso da API

Abra o projeto Usuario presente na pasta API.

Selecione UsuarioApi.Api e execute o projeto em Development.

OBS: Não esqueça de apontar para algum Banco de Dados
no appsettings.json

## Criação da Tabela USUARIO

```sql
CREATE TABLE USUARIO (
	ID INT IDENTITY(1,1) NOT NULL,
	NOME VARCHAR(100) NOT NULL,
	SOBRENOME VARCHAR(30) NOT NULL,
	EMAIL VARCHAR(100),
	DTNASCIMENTO DATETIME NOT NULL,
	ESCOLARIDADE INT NOT NULL
)
```