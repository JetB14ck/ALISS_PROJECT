﻿using Log4NetLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ALISS.Data.Client
{
    public class ApiHelper
    {
        private static readonly ILogService log = new LogService(typeof(ApiHelper));

        public HttpClient _httpClient;// { get; private set; }

        public ApiHelper(string apiUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(apiUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            //_httpClient = httpClient;
        }

        public async Task<List<T>> GetDataListAsync<T>(string apiName)
        {
            log.MethodStart();
            var response = await _httpClient.GetAsync(apiName);
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();

            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<List<T>>(responseContent);
        }

        public async Task<List<T>> GetDataListByParamsAsync<T>(string apiName, string param)
        {
            log.MethodStart();
            var response = await _httpClient.GetAsync($"{apiName}/{param}");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<List<T>>(responseContent);
        }

        public async Task<T> GetDataByIdAsync<T>(string apiName, string obj_id)
        {
            try
            {
                log.MethodStart();
                var response = await _httpClient.GetAsync($"{apiName}/{obj_id}");
                response.EnsureSuccessStatusCode();

                using var responseContent = await response.Content.ReadAsStreamAsync();
                log.MethodFinish();
                return await JsonSerializer.DeserializeAsync<T>(responseContent);
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> PostListofDataAsync<T>(string apiName, List<T> model)
        {
            log.MethodStart();
            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<T>(responseContent);
        }
        public async Task<T> PostDataAsync<T>(string apiName, T model)
        {
            log.MethodStart();
            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<T>(responseContent);
        }
        public async Task<T> PostDataByListAsync<T>(string apiName, string[] model)
        {
            log.MethodStart();

            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<T>(responseContent);
        }
        public async Task<List<T>> PostDataByListModelAsync<T>(string apiName, List<T> model)
        {
            log.MethodStart();

            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<List<T>>(responseContent);
        }

        public async Task<T> GetDataByModelAsync<T, TT>(string apiName, TT model)
        {
            log.MethodStart();
            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<T>(responseContent);
        }

        public async Task<List<T>> GetDataListByModelAsync<T, TT>(string apiName, TT model)
        {
            log.MethodStart();
            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            log.MethodFinish();
            return await JsonSerializer.DeserializeAsync<List<T>>(responseContent);
        }

        public async Task<string> ExportDataAsync<T>(string apiName, FileInfo outputFile, T model)
        {
            log.MethodStart();
            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();
            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = File.Create(outputFile.FullName);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);
            log.MethodFinish();
            return response.StatusCode.ToString();
        }

        public async Task<string> ExportDataBarcodeAsync<T>(string apiName, FileInfo outputFile, T model)
        {
            log.MethodStart();
            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();
            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = File.Create(outputFile.FullName);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);
            log.MethodFinish();
            return "OK";
        }

        public async Task<string> ExportToPdfDataAsync(string apiName, FileInfo outputFile, string[] model)
        {
            log.MethodStart();

            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            //var response = await _httpClient.GetAsync($"{apiName}/{FileName}/{FileExtension}");
            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();
            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = File.Create(outputFile.FullName);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);

            log.MethodFinish();
            return response.StatusCode.ToString();
        }

        public async Task<string> ConvertResultToPdfDataAsync(string apiName, string[] model)
        {
            log.MethodStart();

            var data = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

            //var response = await _httpClient.GetAsync($"{apiName}/{FileName}/{FileExtension}");
            var response = await _httpClient.PostAsync($"{apiName}", content);
            response.EnsureSuccessStatusCode();
            await using var ms = await response.Content.ReadAsStreamAsync();
            //await using var fs = File.Create(outputFile.FullName);
            //ms.Seek(0, SeekOrigin.Begin);
            //ms.CopyTo(fs);

            log.MethodFinish();
            return response.StatusCode.ToString();
        }

    }
}
