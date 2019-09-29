
#include <Expression.h>
#include <PredefinedFunctions.h>
#include <iostream>

using namespace Zorvan::Framework::MathParser;

void main()
{
	Expression exp("((50*(1/(1+(10^((DTrophy-ATrophy)/400)))))*(-0.4))");


	std::cout << exp.ToString();

	std::cout << exp.Calculate({ new Argument("ATrophy", 0), new Argument("DTrophy", 0), new Argument("BSTAR", 0) }) << std::endl;
}