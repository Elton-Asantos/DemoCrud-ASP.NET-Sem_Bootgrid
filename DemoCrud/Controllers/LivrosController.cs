using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemoCrud.AcessoDados;
using DemoCrud.Models;

// Define o namespace do controlador
namespace DemoCrud.Models
{
    public class LivrosController : Controller
    {
        // Instância do contexto do banco de dados para interagir com a tabela "Livros"
        private LivrosContexto db = new LivrosContexto();

        // GET: Livros
        public ActionResult Index()
        {
            // Retorna a view da página inicial dos livros
            return View();
        }

        // GET: Listar livros paginados (PartialView)
        public PartialViewResult Listar(Livro livro, int pagina = 1, int registros = 5)
        {
            // Obtém todos os livros com seus respectivos gêneros
            var livros = db.Livros.Include(l => l.Genero);

            if (!String.IsNullOrWhiteSpace(livro.Titulo))
            {
                livros = livros.Where(l => l.Titulo.Contains(livro.Titulo));
            }

            if (!String.IsNullOrWhiteSpace(livro.Autor))
            {
                livros = livros.Where(l => l.Autor.Contains(livro.Autor));
            }

            if (livro.AnoEdicao != 0)
            {
                livros = livros.Where(l => l.AnoEdicao == livro.AnoEdicao);
            }

            if (livro.Valor != decimal.Zero)
            {
                livros = livros.Where(l => l.Valor == livro.Valor);
            }

            // Aplica ordenação por título e faz a paginação
            var livrosPaginados = livros.OrderBy(l => l.Titulo)
            .Skip((pagina - 1) * registros) // Pula os primeiros registros conforme a página
            .Take(registros); // Seleciona a quantidade de registros desejada

            // Retorna a PartialView chamada "_Listar" com os livros paginados
            return PartialView("_Listar", livrosPaginados.ToList());
        }


        // GET: Detalhes do livro (exibe informações de um livro específico)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                // Retorna um erro HTTP 400 (Bad Request) caso o ID não seja fornecido
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca o livro pelo ID fornecido
            Livro livro = db.Livros.Find(id);

            if (livro == null)
            {
                // Retorna erro HTTP 404 (Not Found) caso o livro não seja encontrado
                return HttpNotFound();
            }

            // Retorna a view com os detalhes do livro
            return View(livro);
        }

        // GET: Criar um novo livro
        public ActionResult Create()
        {
            // Preenche a ViewBag com a lista de gêneros para o dropdown na view
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome");
            return View();
        }

        // POST: Criar um novo livro (processa os dados do formulário)
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção contra ataques CSRF
        public ActionResult Create([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                // Adiciona o livro ao banco de dados e salva as alterações
                db.Livros.Add(livro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Caso ocorra um erro, recarrega a lista de gêneros e retorna à view
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Editar um livro existente
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca o livro pelo ID fornecido
            Livro livro = db.Livros.Find(id);

            if (livro == null)
            {
                return HttpNotFound();
            }

            // Preenche a ViewBag com a lista de gêneros
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return View(livro);
        }

        // POST: Editar um livro existente (processa os dados do formulário)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                // Atualiza o estado do livro no banco e salva as alterações
                db.Entry(livro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Caso ocorra um erro, recarrega a lista de gêneros e retorna à view
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Excluir um livro
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca o livro pelo ID fornecido
            Livro livro = db.Livros.Find(id);

            if (livro == null)
            {
                return HttpNotFound();
            }

            // Retorna a view com o livro a ser excluído
            return View(livro);
        }

        // POST: Confirmação da exclusão de um livro
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Busca o livro pelo ID e remove do banco de dados
            Livro livro = db.Livros.Find(id);
            db.Livros.Remove(livro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Libera os recursos do contexto do banco de dados
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
