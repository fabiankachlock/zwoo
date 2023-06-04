    // /// <summary>
    // /// Store a contact form submission from the frontend in the database
    // /// </summary>
    // /// <param name="data">the submission data</param>
    // /// <returns>the created entity</returns>
    // public ContactRequest CreateContactRequest(ContactForm data)
    // {
    //     ContactRequest request = new ContactRequest()
    //     {
    //         Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    //         Name = data.Name,
    //         Email = data.Email,
    //         Message = data.Message,
    //         CaptchaScore = data.CaptchaScore,
    //         Origin = data.Site
    //     };
    //     _contactRequestsCollection.InsertOne(request);
    //     return request;
    // }