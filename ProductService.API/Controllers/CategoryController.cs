using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTOs;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;

namespace ProductService.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<List<CategoryDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return Ok(APIResponse<List<CategoryDTO>>.SuccessResponse(categories));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error."));
            }
        }
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<CategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(APIResponse<string>.FailResponse(($"Category with id '{id}' not found")));
                }
                return Ok(APIResponse<CategoryDTO>.SuccessResponse(category));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error."));
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<CategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO createDTO)
        {
            try
            {
                var result = await _categoryService.AddAsync(createDTO);
                if (result == null)
                {
                    return StatusCode(500, APIResponse<string>.FailResponse("Failed to create category"));
                }
                return Ok(APIResponse<CategoryDTO>.SuccessResponse(result, "Category created successfully"));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error."));
            }
        }
        [HttpPut]
        [ProducesResponseType(typeof(APIResponse<CategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromBody] CategoryUpdateDTO updateDTO)
        {
            try
            {
                var result = await _categoryService.UpdateAsync(updateDTO);
                if (result == null)
                {
                    return StatusCode(500, APIResponse<string>.FailResponse("Failed to create category"));
                }
                return Ok(APIResponse<CategoryDTO>.SuccessResponse(result, "Category created successfully"));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error."));
            }
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var isDeleted = await _categoryService.DeleteAsync(id);
                if (isDeleted)
                {
                    return Ok(APIResponse<string>.SuccessResponse(string.Empty, "Category deleted successfully"));
                }
                return NotFound(APIResponse<string>.FailResponse("Category Not Found"));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error."));
            }
        }
    }
}
