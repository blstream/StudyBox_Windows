﻿using StudyBox.Core.Models;
using StudyBox.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Net.Http.Headers;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace StudyBox.Core.Services
{
    public class RestService : IRestService
    {
        private ResourceDictionary _resources = Application.Current.Resources;
        private readonly IDeserializeJsonService _deserializeJsonService;
        private readonly IAccountService _accountService;

        public RestService(IDeserializeJsonService deserializeJsonService, IAccountService accountService)
        {
            _deserializeJsonService = deserializeJsonService;
            _accountService = accountService;
        }

        #region public methods

        public async Task<List<Flashcard>> GetFlashcards(string deckId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["FlashcardGetAllUrl"].ToString(), deckId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<List<Flashcard>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Flashcard>> GetFlashcardsWithTipsCount(string deckId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["FlashcardWithTipsCountGetAllUrl"].ToString(), deckId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<List<Flashcard>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Flashcard> GetFlashcardById(string deckId, string flashcardId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["FlashcardGetByIdUrl"].ToString(), deckId, flashcardId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<Flashcard>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Flashcard> CreateFlashcard(Flashcard flashcard, string deckId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["FlashcardCreateUrl"].ToString(), deckId);

            return await CreateHelper<Flashcard>(url,
                new { question = flashcard.Question, answer = flashcard.Answer, isHidden = flashcard.IsHidden },
                true,
                cts);
        }

        public async Task<bool> UpdateFlashcard(Flashcard flashcard, string deckId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["FlashcardUpdateUrl"].ToString(), deckId, flashcard.Id);

            return await UpdateHelper(url,
                new { question = flashcard.Question, answer = flashcard.Answer, isHidden = flashcard.IsHidden },
                cts);
        }


        public async Task<bool> RemoveFlashcard(string deckId, string flashcardId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["FlashcardRemoveUrl"].ToString(), deckId, flashcardId);
            return await RemoveHelper(url, cts);
        }





        public async Task<List<Deck>> GetDecks(CancellationTokenSource cts = null)
        {
            try
            {
                string url = _resources["DeckGetAllUrl"].ToString();
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<List<Deck>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Deck>> GetUserDecks(CancellationTokenSource cts = null)
        {
            try
            {
                string url = _resources["GetAllUserDecks"].ToString();
                string webPageSource = await GetWebPageSource(url, cts, true);

                return _deserializeJsonService.GetObjectFromJson<List<Deck>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Deck> GetDeckById(string deckId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["DeckGetByIdUrl"].ToString(), deckId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<Deck>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Deck>> GetDecksByName(string name, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["DeckGetAllByNameUrl"].ToString(), name);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<List<Deck>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Deck> CreateDeck(Deck deck, CancellationTokenSource cts = null)
        {
            string url = _resources["DeckCreateUrl"].ToString();

            return await CreateHelper<Deck>(url,
                new { name = deck.Name, isPublic = deck.IsPublic },
                true,
                cts);
        }

        public async Task<bool> UpdateDeck(Deck deck, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["DeckUpdateUrl"].ToString(), deck.ID);
            return await UpdateHelper(url,
                new { name = deck.Name, isPublic = deck.IsPublic },
                cts);
        }

        public async Task<bool> RemoveDeck(string deckId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["DeckRemoveUrl"].ToString(), deckId);
            return await RemoveHelper(url, cts);
        }

        public async Task<List<Deck>> GetAllDecks(bool authorize, bool includeOwn, bool flashcardsCount, string name, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["GetAllDecksUrl"].ToString(), includeOwn.ToString(), flashcardsCount.ToString(), name);
                string webPageSource = await GetWebPageSource(url, cts, authorize);

                return _deserializeJsonService.GetObjectFromJson<List<Deck>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Deck> GetRandomDeck(CancellationTokenSource cts = null)
        {
            try
            {
                string url = _resources["GetRandomDeckUrl"].ToString();
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<Deck>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Tip>> GetTips(string deckId, string flashcardId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["TipGetAllUrl"].ToString(), deckId, flashcardId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<List<Tip>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tip> GetTipById(string deckId, string flashcardId, string tipId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["TipGetByIdUrl"].ToString(), deckId, flashcardId, tipId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<Tip>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<Tip> CreateTip(Tip tip, string deckId, string flashcardId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["TipCreateUrl"].ToString(), deckId, flashcardId);

            return await CreateHelper<Tip>(url,
                new { essence = tip.Essence, difficult = tip.Difficult },
                true,
                cts);
        }

        public async Task<bool> UpdateTip(Tip tip, string deckId, string flashcardId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["TipUpdateUrl"].ToString(), deckId, flashcardId, tip.ID);

            return await UpdateHelper(url,
                new { essence = tip.Essence, difficult = tip.Difficult },
                cts);
        }

        public async Task<bool> RemoveTip(string deckId, string flashcardId, string tipId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["TipRemoveUrl"].ToString(), deckId, flashcardId, tipId);
            return await RemoveHelper(url, cts);
        }



        public async Task<List<Tag>> GetTags(string deckId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["TagGetAllUrl"].ToString(), deckId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<List<Tag>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tag> GetTagById(string deckId, string tagId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["TagGetByIdUrl"].ToString(), deckId, tagId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<Tag>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tag> CreateTag(Tag tag, string deckId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["TagCreateUrl"].ToString(), deckId);

            return await CreateHelper<Tag>(url,
                new { name = tag.Name },
                true,
                cts);
        }

        public async Task<bool> UpdateTag(Tag tag, string deckId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["TagUpdateUrl"].ToString(), deckId, tag.ID);

            return await UpdateHelper(url,
                new { name = tag.Name },
                cts);
        }

        public async Task<bool> RemoveTag(string deckId, string tagId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["TagRemoveUrl"].ToString(), deckId, tagId);
            return await RemoveHelper(url, cts);
        }



        public async Task<List<TestResult>> GetTestResults(string deckId, CancellationTokenSource cts = null)
        {
            try
            {
                string url = String.Format(_resources["TestResultsGetAllUrl"].ToString(), deckId);
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<List<TestResult>>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> SaveTestResults(List<TestResult> testResults, string deckId, CancellationTokenSource cts = null)
        {
            string url = String.Format(_resources["TestResultsSaveUrl"].ToString(), deckId);

            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response;

                    if (cts != null)
                    {
                        response = await client.PostAsJsonAsync(url, testResults, cts.Token);
                    }
                    else
                    {
                        response = await client.PostAsJsonAsync(url, testResults);
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.Created && response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return false;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<User> CreateUser(User user, CancellationTokenSource cts = null)
        {
            string url = _resources["UserCreateUrl"].ToString();

            return await CreateHelper<User>(url,
                new { email = user.Email, name = user.Email, password = user.Password },
                false,
                cts);
        }

        public async Task<User> GetLoggedUser(CancellationTokenSource cts = null)
        {
            try
            {
                string url = _resources["UserGetLoggedUrl"].ToString();
                string webPageSource = await GetWebPageSource(url, cts);

                return _deserializeJsonService.GetObjectFromJson<User>(webPageSource);
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UploadFile(StorageFile file, CancellationTokenSource cts = null)
        {
            string url;

            if (file.ContentType.Contains("image"))
                url = String.Format(_resources["FileUploadUrl"].ToString(), "image");
            else
                url = String.Format(_resources["FileUploadUrl"].ToString(), "text");

            try
            {
                byte[] byteArray = await GetFileBytesAsync(file);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", _accountService.GetUserEmail(), _accountService.GetUserPassword()))));

                    MultipartFormDataContent form = new MultipartFormDataContent("---BOUNDARY");
                    HttpContent content = new ByteArrayContent(byteArray);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = file.Name };
                    content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    form.Add(content);

                    HttpResponseMessage response;

                    if (cts != null)
                    {
                        response = await client.PostAsync(url, form, cts.Token);
                    }
                    else
                    {
                        response = await client.PostAsync(url, form);
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    {
                        return false;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<ResetPassword> ResetPassword(string email, CancellationTokenSource cts = null)
        {
            string url = _resources["ResetPasswordUrl"].ToString();

            return await CreateHelper<ResetPassword>(url,
                new { email = email },
                false,
                cts,
                true);
        }

        public async Task<bool> ChangePassword(User user, string token, CancellationTokenSource cts = null)
        {
            string url = _resources["ChangePasswordUrl"].ToString();

            return await CreateHelper<bool>(url,
                new { password = user.Password, token = token, email = user.Email },
                false, cts);
        }
        #endregion


        #region private methods

        private async Task<string> DecodeResponseContent(HttpResponseMessage response)
        {
            string jsonString = "";
            byte[] byteContent = await response.Content.ReadAsByteArrayAsync();

            try
            {
                string encoding = response.Content.Headers.ContentType.CharSet;
                jsonString = Encoding.GetEncoding(encoding).GetString(byteContent);
            }
            catch
            {
                jsonString = Encoding.UTF8.GetString(byteContent);
            }
            return jsonString;
        }

        private async Task<string> GetWebPageSource(string url, CancellationTokenSource cts, bool authorize = false)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                if (authorize)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", _accountService.GetUserEmail(), _accountService.GetUserPassword()))));
                }

                HttpResponseMessage response;
                if (cts == null)
                {
                    response = await httpClient.GetAsync(url);
                }
                else
                {
                    response = await httpClient.GetAsync(url, cts.Token);
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }

                return await DecodeResponseContent(response);
            }
        }



        private async Task<T> CreateHelper<T>(string url, object apiCreateObject, bool authorize, CancellationTokenSource cts, bool decodeError = false)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (authorize)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", _accountService.GetUserEmail(), _accountService.GetUserPassword()))));
                    }

                    HttpResponseMessage response;
                    if (cts == null)
                    {
                        response = await client.PostAsJsonAsync(url, apiCreateObject);
                    }
                    else
                    {
                        response = await client.PostAsJsonAsync(url, apiCreateObject, cts.Token);
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.Created && response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (!decodeError)
                            throw new HttpRequestException();
                        else
                        {
                            string errorDecode = await DecodeResponseContent(response);
                            return _deserializeJsonService.GetObjectFromJson<T>(errorDecode);
                        }
                    }

                    if (typeof(T) == typeof(bool))
                        return (T)(object)true;

                    string json = await DecodeResponseContent(response);
                    return _deserializeJsonService.GetObjectFromJson<T>(json);
                }
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return default(T);
            }
        }


        private async Task<bool> UpdateHelper(string url, object apiUpdateObject, CancellationTokenSource cts)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", _accountService.GetUserEmail(), _accountService.GetUserPassword()))));

                    HttpResponseMessage response;
                    if (cts == null)
                    {
                        response = await client.PutAsJsonAsync(url, apiUpdateObject);
                    }
                    else
                    {
                        response = await client.PutAsJsonAsync(url, apiUpdateObject, cts.Token);
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {
                        throw new HttpRequestException();
                    }

                    return true;
                }
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private async Task<bool> RemoveHelper(string url, CancellationTokenSource cts)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", _accountService.GetUserEmail(), _accountService.GetUserPassword()))));

                    HttpResponseMessage response;
                    if (cts == null)
                    {
                        response = await client.DeleteAsync(url);
                    }
                    else
                    {
                        response = await client.DeleteAsync(url, cts.Token);
                    }

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new HttpRequestException();
                    }

                    return true;
                }
            }
            catch (TaskCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return false;
            }
        }



        private async Task<byte[]> GetFileBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }

        #endregion
    }
}
