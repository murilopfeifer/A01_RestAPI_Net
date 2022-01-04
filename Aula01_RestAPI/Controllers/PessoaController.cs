using Aula01_RestAPI.Data;
using Aula01_RestAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


//Dica:Criar varias controllers. Uma controller para cada entidade, seria um ponto de partida;


namespace Aula01_RestAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
        [HttpGet]
        [Route("pessoas")]
        public async Task<IActionResult> getAllAsync
            (
            [FromServices] Contexto contexto
            )
        {
            var pessoas = await contexto
                 .Pessoas
                 .AsNoTracking() //Só pode ser utilizado em consultas. Altamente recomendado por questões de desempenho;
                 .ToListAsync();
            return pessoas == null ? /*então*/ NotFound() : /*senão*/ Ok(pessoas);
        }

        [HttpGet]
        [Route("pessoas/{id}")]
        public async Task<IActionResult> getByIdAsync
            (
            [FromServices] Contexto contexto,
            [FromRoute] int id
            )
        {
            var pessoa = await contexto
                .Pessoas.AsNoTracking()
                .FirstOrDefaultAsync(p => p.id == id);

            return pessoa == null ? NotFound() : Ok(pessoa);
        }

        [HttpPost]
        [Route("pessoas")]
        public async Task<IActionResult> PostAsync
            (
            [FromServices] Contexto contexto,
            [FromBody] Pessoa pessoa
            )
        {
            if (!ModelState.IsValid) //Verifica se esta sendo validado conforme a model 
            {
                return BadRequest();
            }

            try
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
                return Created($"api/pessoa/{pessoa.id}", pessoa);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        [HttpPut]
        [Route("pessoas/{id}")]
        public async Task<IActionResult> PutAsync
            (
                [FromServices] Contexto contexto,
                [FromBody] Pessoa pessoa,
                [FromRoute] int id
            )
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var p = await contexto.Pessoas
                .FirstOrDefaultAsync(x => x.id == id);

            if (p == null)
                return NotFound();

            try
            {
                p.nome = pessoa.nome;

                contexto.Pessoas.Update(p);
                await contexto.SaveChangesAsync();
                return Ok(p);   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete]
        [Route("pessoas/{id}")]
        public async Task<IActionResult> DeleteAsync
            (
                [FromServices] Contexto contexto,
                [FromRoute] int id
            )
        {
            var p = await contexto.Pessoas.FirstOrDefaultAsync(x => x.id == id);

            if (p == null)
                return BadRequest();

            try
            {
                contexto.Pessoas.Remove(p);
                await contexto.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
