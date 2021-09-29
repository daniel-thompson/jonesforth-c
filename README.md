Jonesforth in C
===============

This is a port of Richard W.M. Jones's "sometimes minimal" Forth
implementation from the original assembler version for Linux / i386. It
requires a C compiler that implements the GNU C labels-as-values
language extension but, other than this single feature, is implemented
using pure ISO C. The result is an extremely well commented Forth
implementation that is almost as portable as C.

*Q: Why are all the comments so misleading?*

Purely because I have not yet had time to rewrite the commentary found
in the Linux / i386 original!

The commentary is, more than anything else, is what makes jonesforth
so noteworthy. For that reason preserving and, where required, updating
the commentary is an important step to complete jonesforth-in-C.
Nevertheless, at present, that update is still on the TODO list.

For now, please be aware that the biggest change between the original
jonesforth and jonesforth-in-C are the changes to the dictionary format.
In particular jonesforth-in-C implements the dictionary using C-style strings
(because that makes it easier to use C literal strings in the VM). Using
C-style strings also allowed us to change the location of the flags
byte so that it is possible to directly convert a codeword pointer back
to a dictionary link pointer.

*Q: Is implementing Jonesforth in C missing the point?*

Maybe.

One of the more beguiling things about Forth is that it can be an
entirely self-sufficient system. It's primitives can be implemented in
assembler (right from the Forth environment if your Forth provides one)
and everything else can be written in Forth. Implementing the primitives
of Forth in C cuts us off from this. However, in exchange, we get code
that is easier for many programmers to read, even if some are a little
freaked out by the scary macros! We also get excellent integration with
existing C libraries, which is pragmatic given this allows us to easily
build on existing C infrastructure. You can see from the CCALL family of
words and the associated constants just how easily the two worlds can be
blended.

This Forth makes modest use of the C library. It would be easy to
make it entirely freestanding but currently we rely on the C library
for I/O (both terminal and file), optimized string functinos and for
memory allocation.

*Q: Why not use pure ISO C? Why use the labels-as-values extension?*

labels-as-values makes it possible to implement the Forth VM using an
approach known as indirect threading, which is the traditional way a
Forth is implemented. In the Forth community "threading" describes the
way new Forth words can call existing Forth words and is a key
contributor to system performance.

Implementing in pure ISO C would prevent us from using direct threading.
In pure C we would have to adopt a slower technique called subroutine
threading where each primitive is it's own function. Do be aware that
the interfaces for the various macros (`NATIVE()`, `NEXT()` and friends)
are designed so that the implementation could be switched to subroutine
threading simply by changing the implementation of these macros (e.g. no
need to rewrite the primitives and built-ins.

Finally be aware that both GNU Compiler Collection and LLVM/clang
implement the required language extension so there will be very few
platforms where it is impossible to find a suitable compiler!
