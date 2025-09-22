# Projeto de Loja Online

Este projeto tem o objetivo de desenvolver habilidades em C# com ASP.NET Core, com foco em backend. Ele consiste numa loja online com as seguintes funcionalidades:

* A página inicial mostra os produtos da loja, com imagem, nome e preço;
* O cliente pode usar uma barra de pesquisa para procurar por algum produto específico;
* O cliente pode criar um novo usuário com um email e senha;
* Cada usuário possui um carrinho com produtos que pretende comprar;
* Cada produto na página inicial pode ser adicionado ao carrinho de um usuário ao clicar num botão. A quantidade a ser adicionada é especificada usando um input;
* Ao adicionar um produto que já havia no carrinho, o carrinho somente registra um aumento de quantidade, evitando duplicidade de informação;
* Há botões para o usuário remover produtos do carrinho de forma individual ou esvaziá-lo;
* Há um botão de "Finalizar compra" que leva o usuário para uma página de checkout;
* Há uma página de checkout que só mostra o carrinho.

Este projeto foi desenvolvido usando as seguintes tecnologias:

* .NET 9;
* ASP.NET Core MVC;
* ASP.NET Core Identity;
* Banco de dados PostegreSQL;
* Entity Framework;
* Dapper.

O ASP.NET Core Identity é utilizado para dar as funcionalidades de criar usuário e login. Ele foi utilizado junto com o Entity Framework. Outras funcionalidades que usam a base dados foram implementadas com Dapper, devido ao controle mais fino que ele proporciona.

# O site

A página inicial mostra todos produtos da loja:

<img width="1920" height="1623" alt="home" src="https://github.com/user-attachments/assets/d12e85db-befc-4e84-97a0-0e60195435b9" />

Há uma barra para pesquisar por produtos. Se pesquisamos por ki, são mostrados todos produtos com "ki" no nome.

<img width="1920" height="878" alt="pesquisar" src="https://github.com/user-attachments/assets/7e744d60-21ff-452a-8986-7a055570472b" />

Ao clicar em Entrar somos direcionados à página de login, que vem do ASP.NET Core Identity:

<img width="1920" height="878" alt="login" src="https://github.com/user-attachments/assets/1cfdf632-fd79-4084-b4f6-efba37d5248a" />

Depois do usuário estar logado, a página inicial passa a mostrar o carrinho dele no canto direito da página:

<img width="1920" height="2342" alt="carrinho" src="https://github.com/user-attachments/assets/1509bf7c-7a78-46dd-9e89-adc5378de69f" />

O carrinho pode ser modificado clicando nos botões de "Adicionar ao carrinho", "Remover" ou "Esvaziar carrinho". Clicar nesses botões modifica a página, atualizando o carrinho, mas sem recarregá-la. Ao clicar no botão "Finalizar compra", o usuário é levado a uma página de checkout. Esta página só mostra o carrinho do usuário:

<img width="1920" height="2236" alt="checkout" src="https://github.com/user-attachments/assets/7ed327bf-3371-4ee1-8324-2cd848f4eea4" />

# Estrutura do projeto

## Models

Os modelos principais são Product e CartEntry. O Product tem as seguintes propriedades:

* uint Id;
* string Name;
* Decimal Price;
* string ImagePath.

Todas informações do produto são guardadas no banco de dados, com exceção da imagem. A propriedade ImagePath guarda a localização da imagem do produto no servidor. As imagens dos produtos estão sendo guardadas na pasta wwwroot/img, mas elas não estão presentes no repositório para não violar direitos autorais.

O modelo CartEntry tem as seguintes propriedades:

* uint Quantity;
* Product Product.

Note que a propriedade Product é um objeto da classe Product. O carrinho de um usuário é modelado como uma coleção de objetos da classe CartEntry. Esses dois modelos são suficientes para conseguir enviar as informações dos produtos e carrinho da base dados para o frontend. Para enviar informações sobre um carrinho para a base dados, não é necessário toda informação do Product, bastando os id's dos produtos. Por isso foi feito um DTO CartEntryDTO que guarda somente uma quantidade e o id de um produto.

Qualquer outro modelo foi gerado automaticamente pelo ASP.NET Core.

## Controllers

Foram criados 2 controllers:

* HomeController;
* UserCartController.

Ambos usam um mesmo padrão de rota, que é:

{controller}/{action}/

Se for necessário enviar alguma informação, ela é enviada por uma query string ou pelo corpo da requisição.

O HomeController é responsável por retornar a página inicial e a página de checkout. Isto corresponde às ações Index e Checkout da classe. A ação Index recebe como entrada uma string query, que é o texto enviado pela barra de pesquisa. Se a página inicial for acessada sem usar a barra de pesquisa, tal query será null. Neste caso é retornada a página inicial com todos os produtos. Caso contrário, são selecionados produtos de acordo com a string query e é retornada a página com somente esses produtos.

O UserCartController é o controller responsável por receber requisições do cliente para manipular seu carrinho. Ele possui 3 ações: Add, RemoveSingleProduct e RemoveAllProducts. A ação Add adiciona uma quantidade de um único produto ao carrinho. A ação RemoveSingleProduct remove do carrinho o produto com o id especificado, sem importar a quantidade. A ação RemoveAllProducts esvazia o carrinho. As 3 ações de UserCartController retornam uma view component do carrinho, para que a página possa ser modificada sem recarregá-la.

A ação Checkout do HomeController e todas as ações do UserCartController necessitam de informações de um usuário. Devido a isso, somente é autorizado o uso dessas ações por um usuário logado. A ação Index do HomeController pode ser acessada sem autorização. Se o usuário estiver logado, a página inicial mostrará seu carrinho, caso contrário não mostrará um carrinho.

## Views

Para o controller Home há views Index e Checkout. Index é a página inicial e Checkout a página de finalizar compra. A página inicial permite ver os produtos da loja e adicioná-los ou removê-los do carrinho. A página de checkout somente mostra quais produtos há no carrinho.

Há também uma view component para o carrinho, localizada em Views/Shared/Components/Cart/Default.cshtml e ViewComponents/CartViewComponent.cs. A view component permite reutilizar o mesmo código em diferentes views. A view component do carrinho é utilizada em ambas views Index e Checkout. É utilizada uma view component no lugar de uma partial view para poder passar parâmetros para ela sem precisar de uma view.

Outras views foram geradas automaticamente pelo ASP.NET MVC ou ASP.NET Core Identity.

## Banco da dados

Foi usado um banco dados PostgreSQL. Foi criada uma tabela "products" em que cada coluna corresponde a uma propriedade do modelo Product. O ASP.NET Core Identity gerou automaticamente diversas tabelas, dentre elas a "AspNetUsers", que contém as informações do usuário, como id, email e senha (PasswordHash). Para guardar na base de dados os carrinhos dos usuários, foi criada uma tabela "cartentries". A tabela tem por colunas o id do usuário (userid), o id do produto (productid) e sua quantidade (quantity). A tabela se chama cartentries devido ao par (productid, quantity) corresponder ao modelo CartEntry. O userid é foreign key que se refere ao id da tabela "AspNetUsers". O carrinho de um usuário não é guardado de forma direta, em vez disso cada linha dessa tabela guarda a informação sobre um produto do carrinho. O carrinho completo de um usuário é obtido selecionando todas linhas com um dado userid. Não é permitido repetir um mesmo produto em diferentes linhas para um mesmo usuário, para diminuir o tamanho da tabela. Para garantir isso, o par (userid, productid) é usado como primary key da tabela.

## Acesso ao banco de dados

O ASP.NET Core Identity, responsável pelos usuários e função de login, acessa o banco de dados usando o Entity Framework. Isso é feito automaticamente pelo ASP.NET Core Identity. As tabelas "products" e "cartentries" são acessadas usando Dapper. Para isso foi criada uma classe estática DataManipulation na pasta Data/Dapper. Ela possui métodos para:

* Selecionar produtos da base de dados;
* Obter as informações do carrinho de algum usuário;
* Adicionar ou remover itens do carrinho de algum usuário.

Esses métodos só permitem acessar ou modificar o carrinho de um usuário se estiver logado em sua conta.

A maioria desse métodos têm queries simples. O método SearchForProduct é especial por ser responsável por procurar produtos relacionados a uma dada string. Ele foi feito para a página inicial mostrar os resultados da pesquisa por um produto. Para isso, esse método procura no banco de dados por produtos cujo nome contenha os mesmos termos enviados pela barra de pesquisa, na mesma ordem em que foram escritos. Nisto foi utilizado o operador LIKE do PostgreSQL, por ter um custo menor de processamento. Como exemplo, se na página inicial é pesquisado por "abc def ghi", a base de dados vai procurar por produtos cujo nome segue o padrão "%abc% %def% %ghi%". Ou seja, os termos "abc", "def" e "ghi" são interpretados como pedaços de palavras que aparecem no nome de um produto.

## Prevenção contra SQL injection

A barra de pesquisa na página inicial recebe uma string que é enviada para o servidor. Essa string é utilizada em queries ao banco de dados. Tendo vindo do cliente, isto introduz um risco de ataque do tipo SQL injection, em que o cliente envia strings com o intuito de executar queries próprias no banco de dados. Para evitar isso, antes de uma query ser feita ao banco de dados, quaisquer strings que são parâmetros dessas queries primeiro passam por testes para saber se são seguras. Somente são permitidas strings que não são nulas, não são muito longas e estão de acordo com uma expressão regular (regex). Do lado do servidor uma query insegura não é executada. Do lado do cliente é feita uma validação do input para que o cliente não escreva inputs inseguros. 

## Uso da API pelo frontend

Todos botões que alteram o carrinho do usuário usam funções definidas em JavaScript que chamam ações do UserCartController. Essas funções estão definidas em /wwwroot/js/cart.js. Ao alterar o carrinho de compras é necessário modificar a página para mostrar o carrinho correto. Para isso as ações do UserCartController retornam uma view component do carrinho que é utilizada para modificar a página sem precisar recarregá-la. O JavaScript recebe uma string com o HTML do carrinho que substitui o innerHTML do elemento com id="cart".
