using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using video_streamming_proxy.Domain;
using video_streamming_proxy.Repository;

namespace video_streamming_proxy.Controllers
{
    public class SignupModel
    {
        [Required(ErrorMessage ="Nome precisa ser informado")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "E-mail deve ser informado")]
        [EmailAddress(ErrorMessage ="E-mail invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha deve ser informado")]
        [DataType(DataType.Password, ErrorMessage ="Senha não atende aos criterios")]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        public User ToUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = this.Name,
                Email = this.Email,
                Password = this.Password,
                Phone = this.Phone,
                UserType = UserType.Default
            };
            return user;
        }
    }
    public class SignupController : Controller
    {
        private readonly IUserRepository userRepository;

        public SignupController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View(new SignupModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SignupModel signupModel)
        {
            Console.WriteLine(signupModel);
            if (!ModelState.IsValid)
            {
                return View("Index", signupModel);
            }
            try
            {
                var user = signupModel.ToUser();
                await userRepository.Save(user);
                ViewBag.SuccessMessage = "Seu cadastro foi efetuado com sucesso";
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);                
                ViewBag.ErrorMessage = "Não foi possível concluir o cadastro. Tente novamente";

            }            
            return View("Index", signupModel);
        }
    }
}
