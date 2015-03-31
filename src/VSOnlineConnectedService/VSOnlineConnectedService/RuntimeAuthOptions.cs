namespace VSOnlineConnectedService
{
    internal enum RuntimeAuthOptions
    {
        /// <summary>
        /// No auth has yet been set
        /// </summary>
        None,
        /// <summary>
        /// Integrated, using Single Sign On (SSO) from their current identity
        /// </summary>
        IntegratedAuth,
        /// <summary>
        /// User is challenged with a Web Dialog, requesting their username and password
        /// </summary>
        BasicAuth,
        /// <summary>
        /// The username/password are embeded in the application, and passed at runtime. 
        /// </summary>
        UsernamePasswordServiceAuth
    };
}
