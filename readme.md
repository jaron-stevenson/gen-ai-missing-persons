# gen-ai-missing-persons
This solution consists of 2 REST APIs a data ingestion REST API that is triggered when  Missing Person PDF files are uploaded to an Azure Blob Container.  When a PDF is uploaded, the Blob Trigger will parse the contains of the PDF file extracting the Text then it will insert that data into SQL.  The 2nd API is the AI Chat REST service, it allows clients to chat with the data. 

# api-process-missing-persons-pdf
This is the Azure Function (Blob Trigger), which gets fired when a Missing Person PDF file is uploaded to an Azure Storage Account.  The function will extract data from the PDF and insert it into SQL.  This is the only purpose of this API.

# api-missing-persons
This is the GenAI Chat Service that allows any Client to interact with the data in a Chat style manner.  It leverages AI to interact with the data in Natural Language manner.  You are able to ask aggregate style questions about the data, examples below.

   ~~~
      Question: How many people went missing between the months of Oct and Dec?
      Response: A total of 3 people went missing during that timeframe.  
   ~~~
