using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam1.Data;
using APIDevSteam1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace APIDevSteam1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<Usuario> _userManager;

        public JogosController(APIContext context, IWebHostEnvironment webHostEnvironment, UserManager<Usuario> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: api/Jogos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jogo>>> GetJogos()
        {
            return await _context.Jogos.ToListAsync();
        }

        // GET: api/Jogos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Jogo>> GetJogo(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);

            if (jogo == null)
            {
                return NotFound();
            }

            return jogo;
        }

        // PUT: api/Jogos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJogo(Guid id, Jogo jogo)
        {
            if (id != jogo.JogoId)
            {
                return BadRequest();
            }

            _context.Entry(jogo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JogoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Jogos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Jogo>> PostJogo(Jogo jogo)
        {
            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJogo", new { id = jogo.JogoId }, jogo);
        }

        // DELETE: api/Jogos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJogo(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
            {
                return NotFound();
            }

            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JogoExists(Guid id)
        {
            return _context.Jogos.Any(e => e.JogoId == id);
        }

        [HttpPost("UploadGameImage")]
        public async Task<IActionResult> UploadGameImage(IFormFile file, Guid jogoId)
        {
            // Verifica se o arquivo é nulo ou vazio
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo não pode ser nulo ou vazio.");

            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null)
                return NotFound("Jogo não encontrado.");

            // Verifica se o arquivo é uma imagem
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("O arquivo deve ser uma imagem.");

            // Define o caminho para salvar a imagem na pasta Resources/Games
            var gamesFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Games");
            if (!Directory.Exists(gamesFolder))
                Directory.CreateDirectory(gamesFolder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, fileExtension) < 0)
                return BadRequest("Formato de arquivo não suportado. Use .jpg, .jpeg, .png ou .gif.");

            var fileName = $"{jogo.JogoId}{fileExtension}";
            var filePath = Path.Combine(gamesFolder, fileName);

            // Verifica se o arquivo já existe e o remove
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Salva o arquivo no disco
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retorna o caminho relativo da imagem
            var relativePath = Path.Combine("Resources", "Games", fileName).Replace("\\", "/");
            return Ok(new { FilePath = relativePath });
        }

        [HttpGet("GetGameImage/{jogoId}")]
        public async Task<IActionResult> GetGameImage(Guid jogoId)
        {
            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null)
                return NotFound("Jogo não encontrado.");

            // Caminho da imagem na pasta Resources/Games
            var gamesFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Games");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            // Procura a imagem do jogo com base no ID
            string? gameImagePath = null;
            foreach (var extension in allowedExtensions)
            {
                var potentialPath = Path.Combine(gamesFolder, $"{jogo.JogoId}{extension}");
                if (System.IO.File.Exists(potentialPath))
                {
                    gameImagePath = potentialPath;
                    break;
                }
            }
            // Se a imagem não for encontrada
            if (gameImagePath == null)
                return NotFound("Imagem do jogo não encontrada.");

            // Lê o arquivo como um array de bytes
            var imageBytes = await System.IO.File.ReadAllBytesAsync(gameImagePath);

            // Converte os bytes para Base64
            var base64Image = Convert.ToBase64String(imageBytes);

            // Retorna a imagem em Base64
            return Ok(new { Base64Image = $"data:image/{Path.GetExtension(gameImagePath).TrimStart('.')};base64,{base64Image}" });
        }

        [HttpPut("UpdateGame")]
        public async Task<IActionResult> UpdateGame([FromBody] Jogo updatedJogo)
        {
            // Verifica se o jogo existe
            var jogo = await _context.Jogos.FindAsync(updatedJogo.JogoId);
            if (jogo == null)
                return NotFound("Jogo não encontrado.");

            // Atualiza os campos permitidos
            jogo.Titulo = updatedJogo.Titulo ?? jogo.Titulo;
            jogo.Preco = updatedJogo.Preco != default ? updatedJogo.Preco : jogo.Preco;
            jogo.Desconto = updatedJogo.Desconto != default ? updatedJogo.Desconto : jogo.Desconto;
            jogo.Banner = updatedJogo.Banner ?? jogo.Banner;
            jogo.Descricao = updatedJogo.Descricao ?? jogo.Descricao;

            // Atualiza o jogo no banco de dados
            _context.Entry(jogo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Jogo atualizado com sucesso!");
        }
    }
}
