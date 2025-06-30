from fastapi.middleware.cors import CORSMiddleware

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # You can restrict this to your frontend URL for more security
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
) 