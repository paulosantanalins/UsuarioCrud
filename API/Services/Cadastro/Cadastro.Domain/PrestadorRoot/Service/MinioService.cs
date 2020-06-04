using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Minio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class MinioService : IMinioService
    {
        private readonly IConfiguration _configuration;

        public MinioService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<byte[]> BuscarPdfByteArray(string nomeArquivo, string bucketName)
        {
            byte[] arquivoByteArray;
            using (MemoryStream ms = new MemoryStream())
            {
                var minio = new MinioClient(_configuration["S3Bucket:endPoint"],
                    _configuration["S3Bucket:accessKey"],
                    _configuration["S3Bucket:secretKey"])
                    .WithSSL();

                await minio.GetObjectAsync(bucketName, nomeArquivo, x => x.CopyTo(ms));                
                arquivoByteArray = ms.ToArray();                
            }

            return arquivoByteArray;
        }

        public async Task<bool> SalvarPdf(string arquivoBase64, string nomeArquivo, string bucketName)
        {
            byte[] pdfBytes = Convert.FromBase64String(arquivoBase64);

            using (MemoryStream ms = new MemoryStream(pdfBytes))
            {
                var minio = new MinioClient(_configuration["S3Bucket:endPoint"],
                    _configuration["S3Bucket:accessKey"],
                    _configuration["S3Bucket:secretKey"])
                    .WithSSL();
                
                try
                {                    
                    await minio.PutObjectAsync(bucketName, nomeArquivo + ".pdf", ms, ms.Length, "application/pdf");
                }
                catch (Exception ex)
                {
                    ms.Close();
                    throw ex;
                }
            }

            return true;
        }

        public async Task<bool> SalvarArquivo(string arquivoBase64, string nomeArquivo, string mimeType, string bucketName)
        {            
            byte[] pdfBytes = Convert.FromBase64String(arquivoBase64);
            using (MemoryStream ms = new MemoryStream(pdfBytes))
            {
                var minio = new MinioClient(_configuration["S3Bucket:endPoint"],
                    _configuration["S3Bucket:accessKey"],
                    _configuration["S3Bucket:secretKey"])
                    .WithSSL();

                try
                {
                    await minio.PutObjectAsync(bucketName, nomeArquivo, ms, ms.Length, mimeType);
                    
                }
                catch (Exception ex)
                {
                    ms.Close();
                    throw ex;
                }
            }

            return true;
        }
    }
}
