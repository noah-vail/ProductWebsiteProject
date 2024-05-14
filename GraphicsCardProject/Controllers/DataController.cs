﻿using GraphicsCardProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GraphicsCardProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        readonly AppDbContext _ctx;
        public DataController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        private async Task<String> GetProductsFromWebAsync()
        {
            string url = "https://raw.githubusercontent.com/noah-vail/JSON/main/proditems.json";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        [HttpGet]
        public async Task<ActionResult<String>> Index()
        {
            DataUtility util = new(_ctx!);
            string payload = "";
            var json = await GetProductsFromWebAsync();

            try
            {
                payload = (await util.LoadCardInfoFromWebToDb(json)) ? "tables loaded" : "problem loading tables";
            }
            catch (Exception ex)
            {
                payload = ex.Message;
            }
            return JsonSerializer.Serialize(payload);
        }
    }
}
