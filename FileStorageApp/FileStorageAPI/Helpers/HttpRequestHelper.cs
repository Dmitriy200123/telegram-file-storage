using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;
using RightServices;

namespace FileStorageAPI.Helpers
{
    public class HttpRequestHelper : IHttpRequestHelper
    {
        private readonly ISenderFormTokenProvider _senderFormTokenProvider;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly IAccessesByUserIdProvider _accessesByUserIdProvider;
        private readonly IInfoStorageFactory _infoStorageFactory;
        
        public HttpRequestHelper(IUserIdFromTokenProvider userIdFromTokenProvider, 
            ISenderFormTokenProvider senderFormTokenProvider, 
            IAccessesByUserIdProvider accessesByUserIdProvider, 
            IInfoStorageFactory infoStorageFactory)
        {
            _userIdFromTokenProvider = userIdFromTokenProvider ?? throw new ArgumentNullException(nameof(userIdFromTokenProvider));
            _senderFormTokenProvider = senderFormTokenProvider ?? throw new ArgumentNullException(nameof(senderFormTokenProvider));
            _accessesByUserIdProvider = accessesByUserIdProvider ?? throw new ArgumentNullException(nameof(accessesByUserIdProvider));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
        }

        /// <inheritdoc />
        public async Task<FileSender> GetNotNullSenderAsync(HttpRequest request)
        {
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            
            if (sender == null)
                throw new InvalidOperationException("Sender not found");

            return sender;
        }

        /// <inheritdoc />
        public async Task<bool> HasAnyFilesAccessAsync(HttpRequest request)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(request, Settings.Key);
            var accesses = await _accessesByUserIdProvider.GetAccessesByUserIdAsync(userId);
            return accesses.Any(access => access == Accesses.ViewAnyFiles);
        }

        /// <inheritdoc />
        public async Task<List<Guid>?> GetUserChats(HttpRequest request)
        {
            var sender = await GetNotNullSenderAsync(request);
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            return hasAnyFilesAccess ? null : sender.Chats.Select(chat => chat.Id).ToList();
        }
    }
}