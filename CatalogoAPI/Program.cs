using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Contexto>( options =>
 {
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoBD"));
 });

var app = builder.Build();


app.UseSwagger();

// Listar todos os produtos
app.MapGet("/Listar", async (Contexto db) => 
{
  try
  {
    var result = await db.Produtos.ToListAsync();
    return Results.Ok(result);
  }
  catch(System.Exception ex)
  {
 return Results.Problem(ex.Message);
  }


});

// Cadastrar um Produto
app.MapPost("/produto", async (Produto produto, Contexto db ) =>
{
   try{
      db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produto/{produto.Id}", produto );
   } catch(System.Exception ex)
  {
 return Results.Problem(ex.Message);
  }

});

//Buscar por ID

app.MapGet("/produtos/{id}", async(int id, Contexto db) =>
{
  var produto = await db.Produtos.FindAsync(id);
  if (produto == null)
  {
    return Results.NotFound();
    
  } 
  return Results.Ok(produto);


});

//Atualizar Produto

app.MapPut("/produtos/{id}", async (int id, Produto produto, Contexto db)=>
{

var AtualizarProduto = await db.Produtos.FindAsync(id);
 if (produto == null)
  {
    return Results.NotFound();
    
  } 

  AtualizarProduto.Nome = produto.Nome;
  AtualizarProduto.Descricao = produto.Descricao;
  AtualizarProduto.Preco= produto.Preco;
  AtualizarProduto.Imagem= produto.Imagem;
  AtualizarProduto.DataCompra= produto.DataCompra;
  AtualizarProduto.Estoque= produto.Estoque;
  AtualizarProduto.CategoraId= produto.CategoraId;

  await db.SaveChangesAsync();
  

  return Results.Ok(produto);

});


// Deletar Produto

app.MapDelete("/produtos/{id}", async (int id, Contexto db) =>
{
  var Deletar = await db.Produtos.FindAsync(id);
  if (Deletar == null)
  {
    return Results.NotFound();
  } 
  db.Produtos.Remove(Deletar);
  await db.SaveChangesAsync();
  return Results.Ok();


});



app.UseSwaggerUI();
app.Run();




	public class Produto
	{
        [Key]
		public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public string? Imagem { get; set; }
        public DateTime DataCompra { get; set; }
        public int Estoque { get; set; }

        public int CategoraId { get; set; }
        public Categoria? Categoria { get; set; }
    }


    	public class Categoria
	{
        [Key]
		public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

    }



  public class Contexto : DbContext
  {
    public Contexto(DbContextOptions<Contexto> options) : base(options)
    {

    }
    public DbSet<Produto> Produtos {get; set;}
    public DbSet< Categoria> Categorias {get; set;}

  }